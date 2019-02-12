// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using static System.FormattableString;

    /// <summary>
    /// Implementation of <see cref="IManageCorrelations" />.
    /// </summary>
    public class CorrelationManager : IManageCorrelations
    {
        private readonly object correlationIdToEvaluatedSubjectMapSync = new object();
        private readonly Dictionary<string, EvaluatedSubject> correlationIdToEvaluatedSubjectMap;
        private readonly object correlationIdToStopwatchMapSync = new object();
        private readonly Dictionary<string, Stopwatch> correlationIdToStopwatchMap;
        private readonly object correlationIdToPositionMapSync = new object();
        private readonly Dictionary<string, int> correlationIdToPositionMap;
        private readonly object additionalCorrelationsSync = new object();
        private IList<IHaveCorrelationId> additionalCorrelations;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationManager"/> class.
        /// </summary>
        public CorrelationManager()
            : this(null, null, null, null)
        {
        }

        private CorrelationManager(
            Dictionary<string, EvaluatedSubject> correlationIdToEvaluatedSubjectMap = null,
            Dictionary<string, Stopwatch> correlationIdToStopwatchMap = null,
            Dictionary<string, int> correlationIdToPositionMap = null,
            IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            this.correlationIdToEvaluatedSubjectMap = correlationIdToEvaluatedSubjectMap ?? new Dictionary<string, EvaluatedSubject>();
            this.correlationIdToStopwatchMap = correlationIdToStopwatchMap ?? new Dictionary<string, Stopwatch>();
            this.correlationIdToPositionMap = correlationIdToPositionMap ?? new Dictionary<string, int>();
            this.additionalCorrelations = additionalCorrelations?.ToList() ?? new List<IHaveCorrelationId>();
        }

        /// <inheritdoc />
        public IManageCorrelations ShallowClone()
        {
            lock (this.correlationIdToEvaluatedSubjectMapSync)
            {
                lock (this.correlationIdToStopwatchMapSync)
                {
                    lock (this.correlationIdToPositionMapSync)
                    {
                        lock (this.additionalCorrelationsSync)
                        {
                            var newCorrelationIdToEvaluatedSubjectMap =
                                this.correlationIdToEvaluatedSubjectMap.ToDictionary(k => k.Key, v => v.Value);
                            var newCorrelationIdToStopwatchMap =
                                this.correlationIdToStopwatchMap.ToDictionary(k => k.Key, v => v.Value);
                            var newCorrelationIdToPositionMap =
                                this.correlationIdToPositionMap.ToDictionary(k => k.Key, v => v.Value);

                            var result = new CorrelationManager(
                                newCorrelationIdToEvaluatedSubjectMap,
                                newCorrelationIdToStopwatchMap,
                                newCorrelationIdToPositionMap);
                            return result;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public void AddSubjectCorrelation(Func<object> correlatingSubjectFunc, string correlationId = null)
        {
            if (correlatingSubjectFunc == null)
            {
                throw new ArgumentNullException(Invariant($"Cannot have a null {nameof(correlatingSubjectFunc)}."));
            }

            var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToUpperInvariant();
            if (this.correlationIdToPositionMap.ContainsKey(localCorrelationId))
            {
                throw new ArgumentException(Invariant($"Cannot add an subject correlation twice with the same key; {nameof(correlationId)}={localCorrelationId}"));
            }

            lock (this.correlationIdToEvaluatedSubjectMapSync)
            {
                var evaluatedSubject = new EvaluatedSubject(correlatingSubjectFunc);
                this.correlationIdToEvaluatedSubjectMap.Add(localCorrelationId, evaluatedSubject);
            }
        }

        /// <inheritdoc />
        public void AddElapsedCorrelation(string correlationId = null)
        {
            var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToUpperInvariant();
            if (this.correlationIdToPositionMap.ContainsKey(localCorrelationId))
            {
                throw new ArgumentException(Invariant($"Cannot add an elapsed correlation twice with the same key; {nameof(correlationId)}={localCorrelationId}"));
            }

            lock (this.correlationIdToStopwatchMapSync)
            {
                var stopWatch = new Stopwatch();
                this.correlationIdToStopwatchMap.Add(localCorrelationId, stopWatch);
            }
        }

        /// <inheritdoc />
        public void AddOrderCorrelation(int startingIndex = 0, string correlationId = null)
        {
            var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToUpperInvariant();
            if (this.correlationIdToPositionMap.ContainsKey(localCorrelationId))
            {
                throw new ArgumentException(Invariant($"Cannot add an order correlation twice with the same key; {nameof(correlationId)}={localCorrelationId}"));
            }

            lock (this.correlationIdToPositionMapSync)
            {
                this.correlationIdToPositionMap.Add(localCorrelationId, startingIndex);
            }
        }

        /// <inheritdoc />
        public void AddAdditionalCorrelations(IReadOnlyCollection<IHaveCorrelationId> newAdditionalCorrelations)
        {
            if (newAdditionalCorrelations != null && newAdditionalCorrelations.Count > 0)
            {
                lock (this.additionalCorrelationsSync)
                {
                    if (this.additionalCorrelations == null)
                    {
                        this.additionalCorrelations = new List<IHaveCorrelationId>();
                    }

                    foreach (var additionalCorrelation in newAdditionalCorrelations)
                    {
                        this.additionalCorrelations.Add(additionalCorrelation);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IHaveCorrelationId> GetNextCorrelations()
        {
            lock (this.correlationIdToEvaluatedSubjectMapSync)
            {
                lock (this.correlationIdToStopwatchMapSync)
                {
                    lock (this.correlationIdToPositionMapSync)
                    {
                        lock (this.additionalCorrelationsSync)
                        {
                            var result = new List<IHaveCorrelationId>();
                            foreach (var evaluatedSubject in this.correlationIdToEvaluatedSubjectMap)
                            {
                                var subjectCorrelation = new SubjectCorrelation(evaluatedSubject.Key, new RawSubject(evaluatedSubject.Value.Subject, evaluatedSubject.Value.SubjectToString).ToSubject());
                                result.Add(subjectCorrelation);
                            }

                            foreach (var stopwatch in this.correlationIdToStopwatchMap)
                            {
                                var elapsed = TimeSpan.Zero;
                                if (stopwatch.Value.IsRunning)
                                {
                                    elapsed = stopwatch.Value.Elapsed;
                                }
                                else
                                {
                                    stopwatch.Value.Start();
                                }

                                var elapsedCorrelation = new ElapsedCorrelation(stopwatch.Key, elapsed);
                                result.Add(elapsedCorrelation);
                            }

                            var positionKeysToAdvance = new List<string>();
                            foreach (var position in this.correlationIdToPositionMap)
                            {
                                positionKeysToAdvance.Add(position.Key);
                                var orderCorrelation = new OrderCorrelation(position.Key, position.Value);
                                result.Add(orderCorrelation);
                            }

                            foreach (var positionKeyToAdvance in positionKeysToAdvance)
                            {
                                var currentValue = this.correlationIdToPositionMap[positionKeyToAdvance];
                                this.correlationIdToPositionMap[positionKeyToAdvance] = currentValue + 1;
                            }

                            result.AddRange(this.additionalCorrelations);

                            return result;
                        }
                    }
                }
            }
        }
    }
}