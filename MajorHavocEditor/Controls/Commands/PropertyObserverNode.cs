//===================================================================================
//The MIT License (MIT)
//Copyright(c) Prism Library
//All rights reserved. Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the "Software"), to deal in the
//Software without restriction, including without limitation the rights to use, copy, modify,
//merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
//persons to whom the Software is furnished to do so, subject to the following conditions: 
//
//The above copyright notice and this permission notice shall be included in all copies
//or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//DEALINGS IN THE SOFTWARE.
//===================================================================================
// Code comes from https://github.com/PrismLibrary/Prism/tree/master/src/Prism.Core/Commands
//===================================================================================
using System;
using System.ComponentModel;
using System.Reflection;

namespace MajorHavocEditor.Controls.Commands
{
    /// <summary>
    /// Represents each node of nested properties expression and takes care of 
    /// subscribing/unsubscribing INotifyPropertyChanged.PropertyChanged listeners on it.
    /// </summary>
    internal class PropertyObserverNode
    {
        private readonly Action _action;
        private INotifyPropertyChanged _inpcObject;

        public PropertyInfo PropertyInfo { get; }
        public PropertyObserverNode Next { get; set; }

        public PropertyObserverNode(PropertyInfo propertyInfo, Action action)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            _action = () =>
            {
                action?.Invoke();
                if (Next == null) return;
                Next.UnsubscribeListener();
                GenerateNextNode();
            };
        }

        public void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
        {
            _inpcObject = inpcObject;
            _inpcObject.PropertyChanged += OnPropertyChanged;

            if (Next != null) GenerateNextNode();
        }

        private void GenerateNextNode()
        {
            var nextProperty = PropertyInfo.GetValue(_inpcObject);
            if (nextProperty == null) return;
            if (!(nextProperty is INotifyPropertyChanged nextInpcObject))
                throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                    $"owns '{Next.PropertyInfo.Name}' property, but the object does not implements INotifyPropertyChanged.");

            Next.SubscribeListenerFor(nextInpcObject);
        }

        private void UnsubscribeListener()
        {
            if (_inpcObject != null)
                _inpcObject.PropertyChanged -= OnPropertyChanged;

            Next?.UnsubscribeListener();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == PropertyInfo.Name || string.IsNullOrEmpty(e?.PropertyName))
            {
                _action?.Invoke();
            }
        }
    }
}
