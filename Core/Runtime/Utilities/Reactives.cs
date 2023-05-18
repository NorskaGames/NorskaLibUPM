using System;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.Utilities
{
    public class ReactiveReference<C> where C : class
    {
        private C value;

        /// <summary>
        /// NOTE: Values are compared using '==' operator.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        public C Value
        {
            get => value;

            set
            {
                if (this.value == value)
                    return;

                this.value = value;
                if (value == null && IsAssigned)
                {
                    IsAssigned = false;
                    onUnassigned?.Invoke();
                }
                else
                {
                    IsAssigned = true;
                    onAssigned?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// Invoked when Value is changed to not null object reference.
        /// </summary>
        public Action<C> onAssigned;
        /// <summary>
        /// Invoked when Value is changed to null.
        /// </summary>
        public Action onUnassigned;

        /// <summary>
        /// Same as "Value != null", yet it is cached.
        /// </summary>
        public bool IsAssigned { get; private set;}

        public ReactiveReference()
        {
            value = null;
            IsAssigned = false;
        }

        public ReactiveReference(C initial)
        {
            value = initial;
            IsAssigned = initial != null;
        }
    }

    public class ReactiveValue<V> where V : struct
    {
        private V value;

        /// <summary>
        /// NOTE: Values are compared using 'EqualityComparer<V>.Default.Equals()' method.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        public V Value
        {
            get => value;

            set
            {
                if (EqualityComparer<V>.Default.Equals(this.value, value))
                    return;

                this.value = value;
                onChanged?.Invoke(value);
            }
        }

        public Action<V> onChanged;

        public ReactiveValue()
        {
            value = default(V);
        }

        public ReactiveValue(V initial)
        {
            value = initial;
        }
    }
}