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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MajorHavocEditor.Controls.Commands
{
    /// <summary>
    /// Provide a way to observe property changes of INotifyPropertyChanged objects and invokes a 
    /// custom action when the PropertyChanged event is fired.
    /// </summary>
    internal class PropertyObserver
    {
        private readonly Action _action;

        private PropertyObserver(Expression propertyExpression, Action action)
        {
            _action = action;
            SubscribeListeners(propertyExpression);
        }

        private void SubscribeListeners(Expression propertyExpression)
        {
            var propNameStack = new Stack<PropertyInfo>();
            while (propertyExpression is MemberExpression temp) // Gets the root of the property chain.
            {
                propertyExpression = temp.Expression;
                propNameStack.Push(temp.Member as PropertyInfo); // Records the member info as property info
            }

            if (!(propertyExpression is ConstantExpression constantExpression))
                throw new NotSupportedException("Operation not supported for the given expression type. " +
                                                "Only MemberExpression and ConstantExpression are currently supported.");

            var propObserverNodeRoot = new PropertyObserverNode(propNameStack.Pop(), _action);
            PropertyObserverNode previousNode = propObserverNodeRoot;
            foreach (var propName in propNameStack) // Create a node chain that corresponds to the property chain.
            {
                var currentNode = new PropertyObserverNode(propName, _action);
                previousNode.Next = currentNode;
                previousNode = currentNode;
            }

            object propOwnerObject = constantExpression.Value;

            if (!(propOwnerObject is INotifyPropertyChanged inpcObject))
                throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                    $"owns '{propObserverNodeRoot.PropertyInfo.Name}' property, but the object does not implements INotifyPropertyChanged.");

            propObserverNodeRoot.SubscribeListenerFor(inpcObject);
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls a custom action on 
        /// property changed notifications. The given expression must be in this form: "() => Prop.NestedProp.PropToObserve".
        /// </summary>
        /// <param name="propertyExpression">Expression representing property to be observed. Ex.: "() => Prop.NestedProp.PropToObserve".</param>
        /// <param name="action">Action to be invoked when PropertyChanged event occurs.</param>
        internal static PropertyObserver Observes<T>(Expression<Func<T>> propertyExpression, Action action)
        {
            return new PropertyObserver(propertyExpression.Body, action);
        }
    }
}
