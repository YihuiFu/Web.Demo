﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Foyerry.Server.Common
{
    public class AOP
    {
        /// <summary>
        /// Chain of aspects to invoke
        /// </summary>
        internal Action<Action> Chain = null;

        /// <summary>
        /// The acrual work delegate that is finally called
        /// </summary>
        internal Delegate WorkDelegate;

        /// <summary>
        /// Create a composition of function e.g. f(g(x))
        /// </summary>
        /// <param name="newAspectDelegate">A delegate that offers an aspect's behavior. 
        /// It's added into the aspect chain</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public AOP Combine(Action<Action> newAspectDelegate)
        {
            if (this.Chain == null)
            {
                this.Chain = newAspectDelegate;
            }
            else
            {
                Action<Action> existingChain = this.Chain;
                Action<Action> callAnother = (work) => existingChain(() => newAspectDelegate(work));
                this.Chain = callAnother;
            }
            return this;
        }

        /// <summary>
        /// Execute your real code applying the aspects over it
        /// </summary>
        /// <param name="work">The actual code that needs to be run</param>
        [DebuggerStepThrough]
        public void Do(Action work)
        {
            if (this.Chain == null)
            {
                work();
            }
            else
            {
                this.Chain(work);
            }
        }

        /// <summary>
        /// Execute your real code applying aspects over it.
        /// </summary>
        /// <typeparam name="TReturnType"></typeparam>
        /// <param name="work">The actual code that needs to be run</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public TReturnType Return<TReturnType>(Func<TReturnType> work)
        {
            this.WorkDelegate = work;

            if (this.Chain == null)
            {
                return work();
            }
            else
            {
                TReturnType returnValue = default(TReturnType);
                this.Chain(() =>
                {
                    Func<TReturnType> workDelegate = WorkDelegate as Func<TReturnType>;
                    returnValue = workDelegate();
                });
                return returnValue;
            }
        }

        /// <summary>
        /// Handy property to start writing aspects using fluent style
        /// </summary>
        public static AOP Define
        {
            [DebuggerStepThrough]
            get
            {
                return new AOP();
            }
        }
    }

    public static class AspectExtensions
    {
        [DebuggerStepThrough]
        public static void DoNothing()
        {
        }

        [DebuggerStepThrough]
        public static void DoNothing(params object[] whatever)
        {
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(1000, 1, (error) => DoNothing(error), x => DoNothing(), work, logger));
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, Action<IEnumerable<Exception>> failHandler, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(1000, 1, (error) => DoNothing(error), x => DoNothing(), work, logger));
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, int retryDuration, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(retryDuration, 1, (error) => DoNothing(error), x => DoNothing(), work, logger));
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, int retryDuration,
            Action<Exception> errorHandler, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(retryDuration, 1, errorHandler, x => DoNothing(), work, logger));
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, int retryDuration,
            int retryCount, Action<Exception> errorHandler, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(retryDuration, retryCount, errorHandler, x => DoNothing(), work, logger));
        }

        [DebuggerStepThrough]
        public static AOP Retry(this AOP aspects, int retryDuration,
            int retryCount, Action<Exception> errorHandler, Action<IEnumerable<Exception>> retryFailed, ILogger logger)
        {
            return aspects.Combine((work) =>
                Retry(retryDuration, retryCount, errorHandler, retryFailed, work, logger));
        }

        [DebuggerStepThrough]
        public static void Retry(int retryDuration, int retryCount,
            Action<Exception> errorHandler, Action<IEnumerable<Exception>> retryFailed, Action work, ILogger logger)
        {
            List<Exception> errors = null;
            do
            {
                try
                {
                    work();
                    return;
                }
                catch (Exception x)
                {
                    if (null == errors)
                        errors = new List<Exception>();
                    errors.Add(x);
                    logger.LogException(x);
                    errorHandler(x);
                    System.Threading.Thread.Sleep(retryDuration);
                }
            } while (retryCount-- > 0);
            retryFailed(errors);
        }

        [DebuggerStepThrough]
        public static AOP Delay(this AOP aspect, int milliseconds)
        {
            return aspect.Combine((work) =>
            {
                System.Threading.Thread.Sleep(milliseconds);
                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP MustBeNonDefault<T>(this AOP aspect, params T[] args)
            where T : IComparable
        {
            return aspect.Combine((work) =>
            {
                T defaultvalue = default(T);
                for (int i = 0; i < args.Length; i++)
                {
                    T arg = args[i];
                    if (arg == null || arg.Equals(defaultvalue))
                        throw new ArgumentException(
                            string.Format("Parameter at index {0} is null", i));
                }

                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP MustBeNonNull(this AOP aspect, params object[] args)
        {
            return aspect.Combine((work) =>
            {
                for (int i = 0; i < args.Length; i++)
                {
                    object arg = args[i];
                    if (arg == null)
                        throw new ArgumentException(
                            string.Format("Parameter at index {0} is null", i));
                }

                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP Until(this AOP aspect, Func<bool> test)
        {
            return aspect.Combine((work) =>
            {
                while (!test()) ;

                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP While(this AOP aspect, Func<bool> test)
        {
            return aspect.Combine((work) =>
            {
                while (test())
                    work();
            });
        }

        [DebuggerStepThrough]
        public static AOP WhenTrue(this AOP aspect, params Func<bool>[] conditions)
        {
            return aspect.Combine((work) =>
            {
                foreach (Func<bool> condition in conditions)
                    if (!condition())
                        return;

                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP Log(this AOP aspect, ILogger logger, string[] categories,
            string logMessage, params object[] arg)
        {
            return aspect.Combine((work) =>
            {
                logger.Log(categories, logMessage);

                work();
            });
        }

        [DebuggerStepThrough]
        public static AOP Log(this AOP aspect, ILogger logger,
            string logMessage, params object[] arg)
        {
            return aspect.Combine((work) =>
            {
                logger.Log(string.Format(logMessage, arg));

                work();
            });
        }


        [DebuggerStepThrough]
        public static AOP Log(this AOP aspect, ILogger logger, string[] categories,
            string beforeMessage, string afterMessage)
        {
            return aspect.Combine((work) =>
            {
                logger.Log(categories, beforeMessage);

                work();

                logger.Log(categories, afterMessage);
            });
        }

        [DebuggerStepThrough]
        public static AOP Log(this AOP aspect, ILogger logger,
            string beforeMessage, string afterMessage)
        {
            return aspect.Combine((work) =>
            {
                logger.Log(beforeMessage);

                work();

                logger.Log(afterMessage);
            });
        }

        [DebuggerStepThrough]
        public static AOP HowLong(this AOP aspect, ILogger logger,
            string startMessage, string endMessage)
        {
            return aspect.Combine((work) =>
            {
                DateTime start = DateTime.Now;
                logger.Log(startMessage);

                work();

                DateTime end = DateTime.Now.ToUniversalTime();
                TimeSpan duration = end - start;

                logger.Log(string.Format(endMessage, duration.TotalMilliseconds,
                    duration.TotalSeconds, duration.TotalMinutes, duration.TotalHours,
                    duration.TotalDays));
            });
        }

        [DebuggerStepThrough]
        public static AOP TrapLog(this AOP aspect, ILogger logger)
        {
            return aspect.Combine((work) =>
            {
                try
                {
                    work();
                }
                catch (Exception x)
                {
                    logger.LogException(x);
                }
            });
        }

        [DebuggerStepThrough]
        public static AOP TrapLogThrow(this AOP aspect, ILogger logger)
        {
            return aspect.Combine((work) =>
            {
                try
                {
                    work();
                }
                catch (Exception x)
                {
                    logger.LogException(x);
                    throw;
                }
            });
        }

        [DebuggerStepThrough]
        public static AOP RunAsync(this AOP aspect, Action completeCallback)
        {
            return aspect.Combine((work) => work.BeginInvoke(asyncresult =>
            {
                work.EndInvoke(asyncresult); completeCallback();
            }, null));
        }

        [DebuggerStepThrough]
        public static AOP RunAsync(this AOP aspect)
        {
            return aspect.Combine((work) => work.BeginInvoke(work.EndInvoke, null));
        }

        [DebuggerStepThrough]
        public static AOP Cache<TReturnType>(this AOP aspect,
            ICache cacheResolver, string key)
        {
            return aspect.Combine((work) =>
            { if (cacheResolver != null) Cache<TReturnType>(aspect, cacheResolver, key, work, cached => cached); });
        }

        [DebuggerStepThrough]
        public static AOP CacheList<TItemType, TListType>(this AOP aspect,
            ICache cacheResolver, string listCacheKey, Func<TItemType, string> getItemKey)
            where TListType : IList<TItemType>, new()
        {
            return aspect.Combine((work) =>
            {
                var workDelegate = aspect.WorkDelegate as Func<TListType>;

                // Replace the actual work delegate with a new delegate so that
                // when the actual work delegate returns a collection, each item
                // in the collection is stored in cache individually.
                Func<TListType> newWorkDelegate = () =>
                {
                    TListType collection = workDelegate();
                    foreach (TItemType item in collection)
                    {
                        string key = getItemKey(item);
                        cacheResolver.Set(key, item);
                    }
                    return collection;
                };
                aspect.WorkDelegate = newWorkDelegate;

                // Get the collection from cache or real source. If collection is returned
                // from cache, resolve each item in the collection from cache
                Cache<TListType>(aspect, cacheResolver, listCacheKey, work,
                    cached =>
                    {
                        // Get each item from cache. If any of the item is not in cache
                        // then discard the whole collection from cache and reload the 
                        // collection from source.
                        TListType itemList = new TListType();
                        foreach (TItemType item in cached)
                        {
                            object cachedItem = cacheResolver.Get(getItemKey(item));
                            if (null != cachedItem)
                            {
                                itemList.Add((TItemType)cachedItem);
                            }
                            else
                            {
                                // One of the item is missing from cache. So, discard the 
                                // cached list.
                                return default(TListType);
                            }
                        }

                        return itemList;
                    });
            });
        }

        [DebuggerStepThrough]
        public static AOP CacheRetry<TReturnType>(this AOP aspect,
            ICache cacheResolver,
            ILogger logger,
            string key)
        {
            return aspect.Combine((work) =>
            {
                try
                {
                    Cache<TReturnType>(aspect, cacheResolver, key, work, cached => cached);
                }
                catch (Exception x)
                {
                    logger.LogException(x);
                    System.Threading.Thread.Sleep(1000);

                    //Retry
                    try
                    {
                        Cache<TReturnType>(aspect, cacheResolver, key, work, cached => cached);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex);
                        throw ex;
                    }
                }
            });
        }

        private static void Cache<TReturnType>(AOP aspect, ICache cacheResolver,
            string key, Action work, Func<TReturnType, TReturnType> foundInCache)
        {
            object cachedData = cacheResolver.Get(key);
            if (cachedData == null)
            {
                GetListFromSource<TReturnType>(aspect, cacheResolver, key);
            }
            else
            {
                // Give caller a chance to shape the cached item before it is returned
                TReturnType cachedType = foundInCache((TReturnType)cachedData);
                if (cachedType == null)
                {
                    GetListFromSource<TReturnType>(aspect, cacheResolver, key);
                }
                else
                {
                    aspect.WorkDelegate = new Func<TReturnType>(() => cachedType);
                }
            }

            work();
        }

        private static void GetListFromSource<TReturnType>(AOP aspect, ICache cacheResolver, string key)
        {
            Func<TReturnType> workDelegate = aspect.WorkDelegate as Func<TReturnType>;
            TReturnType realObject = workDelegate();
            cacheResolver.Add(key, realObject);
            workDelegate = () => realObject;
            aspect.WorkDelegate = workDelegate;
        }


    }

}
