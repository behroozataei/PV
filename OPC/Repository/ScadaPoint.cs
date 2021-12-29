using System;

namespace OPC
{
    public enum SinglePointStatus
    {
        Disappear = 0,
        Appear = 1
    }

    public sealed class ScadaPoint
    {
        public ScadaPoint(Guid id, float value)
        {
            Id = id;
            Value = value;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Quality { get; set; }
        public float Value { get; set; }

    }

    public enum DigitalStatus
    {
        Intransit = 0,
        Open = 1,
        Close = 2,
        Disturb = 3
    }
}
