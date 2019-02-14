﻿// --------------------------------------------------------------------------------------------------------------------
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
    using OBeautifulCode.Error.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Implementation of <see cref="IManageCorrelations" />.
    /// </summary>
    public class CorrelationManager : IManageCorrelations
    {
        private readonly object subjectCorrelationsSync = new object();
        private readonly List<ManagedCorrelation<EvaluatedSubject>> subjectCorrelations;
        private readonly object stopwatchCorrelationsSync = new object();
        private readonly List<ManagedCorrelation<Stopwatch>> stopwatchCorrelations;
        private readonly object positionCorrelationsSync = new object();
        private readonly List<ManagedCorrelation<int>> positionCorrelations;
        private readonly object additionalCorrelationsSync = new object();
        private string errorCodeKeyField = Constants.ExceptionDataKeyForErrorCode;
        private string exceptionCorrelationsCorrelationId;
        private IList<IHaveCorrelationId> additionalCorrelations;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationManager"/> class.
        /// </summary>
        public CorrelationManager()
            : this(null, null, null, null)
        {
        }

        private CorrelationManager(
            List<ManagedCorrelation<EvaluatedSubject>> subjectCorrelations = null,
            List<ManagedCorrelation<Stopwatch>> stopwatchCorrelations = null,
            List<ManagedCorrelation<int>> positionCorrelations = null,
            IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            this.subjectCorrelations = subjectCorrelations ?? new List<ManagedCorrelation<EvaluatedSubject>>();
            this.stopwatchCorrelations = stopwatchCorrelations ?? new List<ManagedCorrelation<Stopwatch>>();
            this.positionCorrelations = positionCorrelations ?? new List<ManagedCorrelation<int>>();
            this.additionalCorrelations = additionalCorrelations?.ToList() ?? new List<IHaveCorrelationId>();
        }

        /// <inheritdoc />
        public IManageCorrelations ShallowClone()
        {
            lock (this.subjectCorrelationsSync)
            {
                lock (this.stopwatchCorrelationsSync)
                {
                    lock (this.positionCorrelationsSync)
                    {
                        lock (this.additionalCorrelationsSync)
                        {
                            var newSubjectCorrelations = this.subjectCorrelations.Select(_ => _).ToList();
                            var newStopwatchCorrelations = this.stopwatchCorrelations.Select(_ => _).ToList();
                            var newPositionCorrelations = this.positionCorrelations.Select(_ => _).ToList();

                            var result = new CorrelationManager(
                                newSubjectCorrelations,
                                newStopwatchCorrelations,
                                newPositionCorrelations);
                            return result;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public void AddSubjectCorrelation(Func<object> correlatingSubjectFunc, string correlationId = null)
        {
            lock (this.subjectCorrelationsSync)
            {
                var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToLowerInvariant();

                var evaluatedSubject = new EvaluatedSubject(correlatingSubjectFunc);
                this.subjectCorrelations.Add(new ManagedCorrelation<EvaluatedSubject>(localCorrelationId, evaluatedSubject));
            }
        }

        /// <inheritdoc />
        public void AddElapsedCorrelation(string correlationId = null)
        {
            lock (this.stopwatchCorrelationsSync)
            {
                var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToLowerInvariant();

                var stopWatch = new Stopwatch();
                this.stopwatchCorrelations.Add(new ManagedCorrelation<Stopwatch>(localCorrelationId, stopWatch));
            }
        }

        /// <inheritdoc />
        public void AddOrderCorrelation(int startingIndex = 0, string correlationId = null)
        {
            lock (this.positionCorrelationsSync)
            {
                var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToLowerInvariant();

                this.positionCorrelations.Add(new ManagedCorrelation<int>(localCorrelationId, startingIndex));
            }
        }

        /// <inheritdoc />
        public void PrepareExceptionCorrelations(string errorCodeKey = Constants.ExceptionDataKeyForErrorCode, string correlationId = null)
        {
            this.errorCodeKeyField = errorCodeKey;
            this.exceptionCorrelationsCorrelationId = correlationId;
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
            lock (this.subjectCorrelationsSync)
            {
                lock (this.stopwatchCorrelationsSync)
                {
                    lock (this.positionCorrelationsSync)
                    {
                        lock (this.additionalCorrelationsSync)
                        {
                            var result = new List<IHaveCorrelationId>();
                            foreach (var evaluatedSubject in this.subjectCorrelations)
                            {
                                var subjectCorrelation = new SubjectCorrelation(evaluatedSubject.CorrelationId, new RawSubject(evaluatedSubject.Item.Subject, evaluatedSubject.Item.SubjectToString).ToSubject());
                                result.Add(subjectCorrelation);
                            }

                            foreach (var stopwatch in this.stopwatchCorrelations)
                            {
                                var elapsed = TimeSpan.Zero;
                                if (stopwatch.Item.IsRunning)
                                {
                                    elapsed = stopwatch.Item.Elapsed;
                                }
                                else
                                {
                                    stopwatch.Item.Start();
                                }

                                var elapsedCorrelation = new ElapsedCorrelation(stopwatch.CorrelationId, elapsed);
                                result.Add(elapsedCorrelation);
                            }

                            foreach (var position in this.positionCorrelations)
                            {
                                var positionItem = position.Item;
                                position.UpdateItem(positionItem + 1);
                                var orderCorrelation = new OrderCorrelation(position.CorrelationId, positionItem);
                                result.Add(orderCorrelation);
                            }

                            result.AddRange(this.additionalCorrelations);

                            return result;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IHaveCorrelationId> GetExceptionCorrelations(Exception exception)
        {
            var result = new List<IHaveCorrelationId>();

            if (exception != null)
            {
                var correlationId = string.IsNullOrWhiteSpace(this.exceptionCorrelationsCorrelationId)
                    ? Guid.NewGuid().ToString().ToLowerInvariant()
                    : this.exceptionCorrelationsCorrelationId;

                var correlatingException = exception.FindFirstExceptionInChainWithExceptionId();
                if (correlatingException == null)
                {
                    exception.GenerateAndWriteExceptionIdIntoExceptionData();
                    correlatingException = exception;
                    var exceptionId = correlatingException.GetExceptionIdFromExceptionData().ToString();
                    var exceptionIdCorrelation = new ExceptionIdCorrelation(correlationId, exceptionId);
                    result.Add(exceptionIdCorrelation);
                }

                var errorCode = exception.GetErrorCode();
                if (!string.IsNullOrWhiteSpace(errorCode))
                {
                    var errorCodeCorrelation = new ErrorCodeCorrelation(
                        correlationId,
                        this.errorCodeKeyField,
                        errorCode);
                    result.Add(errorCodeCorrelation);
                }
            }

            return result;
        }

        private class ManagedCorrelation<T>
        {
            public ManagedCorrelation(string correlationId, T item)
            {
                this.CorrelationId = correlationId;
                this.Item = item;
            }

            public string CorrelationId { get; private set; }

            public T Item { get; private set; }

            public void UpdateItem(T newItem)
            {
                this.Item = newItem;
            }
        }
    }
}