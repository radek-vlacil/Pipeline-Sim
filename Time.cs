using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSim
{
    internal readonly struct Time
    {
        public static Time Min = new() { Milliseconds = 0 };
        public static Time Max = new() { Milliseconds = int.MaxValue };

        public int Milliseconds { get; init; }

        public static bool operator <(Time a, Time b)
        {
            return a.Milliseconds < b.Milliseconds;
        }

        public static bool operator >(Time a, Time b)
        {
            return a.Milliseconds > b.Milliseconds;
        }

        public static bool operator <=(Time a, Time b)
        {
            return a.Milliseconds <= b.Milliseconds;
        }

        public static bool operator >=(Time a, Time b)
        {
            return a.Milliseconds >= b.Milliseconds;
        }

        public static bool operator ==(Time a, Time b)
        {
            return a.Milliseconds == b.Milliseconds;
        }

        public static bool operator !=(Time a, Time b)
        {
            return a.Milliseconds != b.Milliseconds;
        }

        public static Time operator +(Time a, int b)
        {
            return new Time { Milliseconds = a.Milliseconds + b };
        }

        public static int operator -(Time a, Time b)
        {
            return a.Milliseconds - b.Milliseconds;
        }

        public override bool Equals(object? obj)
        {
            return obj is Time time &&
                   Milliseconds == time.Milliseconds;
        }

        public override string ToString()
        {
            return $"{Milliseconds}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Milliseconds);
        }

        public static implicit operator Time(int milliseconds)
        {
            return new Time { Milliseconds = milliseconds };
        }
    }
}
