﻿// *******************************************************
// * <copyright file="SimpleContainer.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class representing the <see cref="SimpleContainer"/>
    /// </summary>
    public class SimpleContainer : IContainer
    {
        /// <summary>
        /// The instance of <see cref="IContainer"/>
        /// </summary>
        public static readonly IContainer Instance = new SimpleContainer();

        /// <summary>
        /// Dictionary containing <see cref="Type"/> mappings
        /// </summary>
        private static readonly IDictionary<Type, Type> Types = new Dictionary<Type, Type>();

        /// <summary>
        /// Dictionary containing <see cref="Type"/> instances
        /// </summary>
        private static readonly IDictionary<Type, object> TypeInstances = new Dictionary<Type, object>();

        /// <inheritdoc/>
        public void Register<TContract, TImplementation>()
        {
            Types[typeof(TContract)] = typeof(TImplementation);
        }

        /// <inheritdoc/>
        public void Register<TContract, TImplementation>(TImplementation instance)
        {
            TypeInstances[typeof(TContract)] = instance;
        }

        /// <inheritdoc/>
        public T Resolve<T>()
        {
            return (T)this.Resolve(typeof(T));
        }

        /// <inheritdoc/>
        private object Resolve(Type contract)
        {
            if (TypeInstances.ContainsKey(contract))
            {
                return TypeInstances[contract];
            }

            Type implementation = Types[contract];
            ConstructorInfo constructor = implementation.GetConstructors()[0];
            ParameterInfo[] constructorParameters = constructor.GetParameters();
            if (constructorParameters.Length == 0)
            {
                return Activator.CreateInstance(implementation);
            }

            List<object> parameters = new List<object>(constructorParameters.Length);
            parameters.AddRange(
                constructorParameters.Select(parameterInfo => this.Resolve(parameterInfo.ParameterType)));
            return constructor.Invoke(parameters.ToArray());
        }
    }
}