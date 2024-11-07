using System;
using System.Collections;
using System.Collections.Generic;
using CloneExtensions;
using UnityEngine;

namespace Framework.Core.UI
{
    public class ViewModel<T>
    {
        private T data;
        public T Data
        {
            get => data;
            set
            {
                data = value;
                onValueChanged?.Invoke(data);
            }
        }
        
        private Action<T> onValueChanged;
        public event Action<T> OnValueChanged
        {
            add => onValueChanged += value;
            remove => onValueChanged -= value;
        }
        public void Init(T data)
        {
            this.data = data;
        }
    }
}
