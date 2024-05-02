﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.CommandLine.Parsing;

/// <summary>
/// The publicly facing class for argument and option data.
/// </summary>
public class ValueResult
{
    private ValueResult(
        CliSymbol valueSymbol,
        object? value,
        IEnumerable<Location> locations,
        ValueResultOutcome outcome,
        // TODO: Error should be an Enumerable<Error> and perhaps should not be here at all, only on ParseResult
        string? error = null)
    {
        ValueSymbol = valueSymbol;
        Value = value;
        Locations = locations;
        Outcome = outcome;
        Error = error;
    }

    /// <summary>
    /// Creates a new ValueResult instance
    /// </summary>
    /// <param name="argument">The CliArgument the value is for.</param>
    /// <param name="value">The entered value.</param>
    /// <param name="locations">The locations list.</param>
    /// <param name="outcome">True if parsing and converting the value was successful.</param>
    /// <param name="error">The CliError if parsing or converting failed, otherwise null.</param>
    internal ValueResult(
            CliArgument argument,
            object? value,
            IEnumerable<Location> locations,
            ValueResultOutcome outcome,
            // TODO: Error should be an Enumerable<Error> and perhaps should not be here at all, only on ParseResult
            string? error = null)
        :this((CliSymbol)argument, value, locations, outcome,error)
    {   }

    /// <summary>
    /// Creates a new ValueResult instance
    /// </summary>
    /// <param name="option">The CliOption the value is for.</param>
    /// <param name="value">The entered value.</param>
    /// <param name="locations">The locations list.</param>
    /// <param name="outcome">True if parsing and converting the value was successful.</param>
    /// <param name="error">The CliError if parsing or converting failed, otherwise null.</param>
    internal ValueResult(
        CliOption option,
        object? value,
        IEnumerable<Location> locations,
        ValueResultOutcome outcome,
        // TODO: Error should be an Enumerable<Error> and perhaps should not be here at all, only on ParseResult
        string? error = null)
    : this((CliSymbol)option, value, locations, outcome, error)
    { }

    /// <summary>
    /// The CliSymbol the value is for. This is always a CliOption or CliArgument.
    /// </summary>
    public CliSymbol ValueSymbol { get; }

    internal object? Value { get; }

    /// <summary>
    /// Returns the value, or the default for the type.
    /// </summary>
    /// <typeparam name="T">The type to return</typeparam>
    /// <returns>The value, cast to the requested type.</returns>
    public T? GetValue<T>()
        => Value is null
            ? default
            : (T?)Value;

    /// <summary>
    /// Gets the locations at which the tokens that made up the value appeared.
    /// </summary>
    /// <remarks>
    /// This needs to be a collection because collection types have multiple tokens and they will not be simple offsets when response files are used.
    /// </remarks>
    public IEnumerable<Location> Locations { get; }

    /// <summary>
    /// True when parsing and converting the value was successful
    /// </summary>
    public ValueResultOutcome Outcome { get; }

    /// <summary>
    /// Parsing and conversion errors when parsing or converting failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Returns test suitable for display. 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<string> TextForDisplay()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> TextForCommandReconstruction()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
        //=> $"{nameof(ValueResult)} ({FormatOutcomeMessage()}) {ValueSymbol?.Name}";
        => $"{nameof(ArgumentResult)} {ValueSymbol.Name}: {string.Join(" ", TextForDisplay())}";

 
    // TODO: This definitely feels like the wrong place for this, (Some completion stuff was stripped out. This was a private method in ArgumentConversionResult
    private string FormatOutcomeMessage()
        => ValueSymbol switch
        {
            CliOption option
                => LocalizationResources.ArgumentConversionCannotParseForOption(Value?.ToString() ?? "", option.Name, ValueSymbolType),
            CliCommand command
                => LocalizationResources.ArgumentConversionCannotParseForCommand(Value?.ToString() ?? "", command.Name, ValueSymbolType),
            //TODO
            _ => throw new NotImplementedException()
        };

    private Type ValueSymbolType
        => ValueSymbol switch
        {
            CliArgument argument => argument.ValueType,
            CliOption option => option.Argument.ValueType,
            _ => throw new NotImplementedException()
        };
}
