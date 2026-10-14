using System;

namespace Core.Events;

public sealed class OrderChangedEventArgs(
    string id, 
    OrderChangeKind kind, 
    decimal delta, 
    decimal totalAfter,
    int linesCount) : EventArgs
{
    public string Id { get; } = id;
    public OrderChangeKind Kind { get; } = kind;
    public decimal Delta { get; } = delta;             
    public decimal TotalAfter { get; } = totalAfter;   
    public int LinesCount { get; } = linesCount;     
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}