namespace Tools;

public readonly record struct Fraction(long Numerator, long Denominator = 1)
{
    public static Fraction operator +(Fraction left, Fraction right)
    {
        var lcm = Numbers.LeastCommonMultiple(left.Denominator, right.Denominator);

        return new Fraction(lcm / left.Denominator * left.Numerator + lcm / right.Denominator * right.Numerator, lcm).Reduce();
    }

    public static Fraction operator -(Fraction left, Fraction right)
    {
        var lcm = Numbers.LeastCommonMultiple(left.Denominator, right.Denominator);

        return new Fraction(lcm / left.Denominator * left.Numerator - lcm / right.Denominator * right.Numerator, lcm).Reduce();
    }

    public static Fraction operator *(Fraction left, Fraction right) => new Fraction(left.Numerator * right.Numerator, left.Denominator * right.Denominator).Reduce();

    public static Fraction operator /(Fraction left, Fraction right) => new Fraction(left.Numerator * right.Denominator, left.Denominator * right.Numerator).Reduce();

    private Fraction Reduce()
    {
        if (IsZero)
            return this;

        long gcd = Numbers.GreatestCommonDivisor(Numerator, Denominator);

        if (gcd == 1)
            return this;

        long numerator = Numerator / gcd;
        long denominator = Denominator / gcd;

        if (denominator < 0)
        {
            numerator *= -1;
            denominator *= -1;
        }

        return new Fraction(numerator, denominator);
    }

    public override string ToString()
    {
        var reduced = Reduce();

        return reduced.Denominator == 1 ? reduced.Numerator.ToString() : $"{reduced.Numerator}/{reduced.Denominator}";
    }

    public bool IsZero => Numerator == 0;

    public bool IsNonZero => !IsZero;
}
