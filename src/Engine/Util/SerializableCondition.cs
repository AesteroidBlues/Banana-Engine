namespace BananaEngine.Util;

using System;
using System.Collections.Generic;

public enum Operator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual
}

public abstract class SerializableCondition<T>
{
    public string ParameterName { get; set; }
    public Operator Operator { get; set; }
    public T ReferenceValue { get; set; }

    public SerializableCondition(string name, Operator op, T value)
    {
        ParameterName = name;
        Operator = op;
        ReferenceValue = value;
    }

    public abstract bool Evaluate(T value);
}

public class FloatCondition : SerializableCondition<float>
{
    public FloatCondition(string name, Operator op, float value)
        : base(name, op, value)
    {
    }

    public override bool Evaluate(float value)
    {
        switch (this.Operator)
        {
            case Operator.Equals:
                return value == ReferenceValue;
            case Operator.NotEquals:
                return value != ReferenceValue;
            case Operator.GreaterThan:
                return value > ReferenceValue;
            case Operator.LessThan:
                return value < ReferenceValue;
            case Operator.GreaterThanOrEqual:
                return value >= ReferenceValue;
            case Operator.LessThanOrEqual:
                return value <= ReferenceValue;
            default:
                throw new Exception("Invalid operator");
        }
    }
}

public class IntCondition
{
    public string ParameterName { get; set; }
    public Operator Operator { get; set; }
    public int ReferenceValue { get; set; }

    public IntCondition(string name, Operator op, int value)
    {
        Operator = op;
        ReferenceValue = value;
    }

    public bool Evaluate(int value)
    {
        switch (Operator)
        {
            case Operator.Equals:
                return value == ReferenceValue;
            case Operator.NotEquals:
                return value != ReferenceValue;
            case Operator.GreaterThan:
                return value > ReferenceValue;
            case Operator.LessThan:
                return value < ReferenceValue;
            case Operator.GreaterThanOrEqual:
                return value >= ReferenceValue;
            case Operator.LessThanOrEqual:
                return value <= ReferenceValue;
            default:
                throw new Exception("Invalid operator");
        }
    }
}

public class StringCondition : SerializableCondition<string>
{
    public StringCondition(string name, Operator op, string value)
        : base(name, op, value)
    {
    }

    public override bool Evaluate(string value)
    {
        switch (Operator)
        {
            case Operator.Equals:
                return value == ReferenceValue;
            case Operator.NotEquals:
                return value != ReferenceValue;
            default:
                throw new Exception("Invalid operator");
        }
    }
}

public class EnumCondition<T> where T : Enum
{
    public string ParameterName { get; set; }
    public Operator Operator { get; set; }
    public T ReferenceValue { get; set; }

    public EnumCondition(string name, Operator op, T value)
    {
        ParameterName = name;
        Operator = op;
        ReferenceValue = value;
    }

    public bool Evaluate(T value)
    {
        switch (Operator)
        {
            case Operator.Equals:
                return value.Equals(ReferenceValue);
            case Operator.NotEquals:
                return !value.Equals(ReferenceValue);
            default:
                throw new Exception("Invalid operator");
        }
    }
}

public class BoolCondition
{
    public string ParameterName { get; set; }
    public bool ReferenceValue { get; set; }

    public BoolCondition(string name, bool value)
    {

        ReferenceValue = value;
    }

    public bool Evaluate(bool value)
    {
        return value == ReferenceValue;
    }
}