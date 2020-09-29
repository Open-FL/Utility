using System;

using Utility.Expressions.Enums;

namespace Utility.Expressions.Interfaces
{
    /// <summary>
    ///     Defines a IEvalValue with additional System.Type and EvalType properties
    /// </summary>
    public interface IEvalTypedValue : IEvalValue
    {

        /// <summary>
        ///     The C# Type of the Value
        /// </summary>
        Type SystemType { get; }

        /// <summary>
        ///     The Evaluator Type of the Value
        /// </summary>
        EvalType EvalType { get; }

    }
}