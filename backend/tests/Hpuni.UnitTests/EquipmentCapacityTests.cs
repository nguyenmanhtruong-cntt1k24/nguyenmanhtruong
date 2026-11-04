using Hpuni.Domain;
using Xunit;

namespace Hpuni.UnitTests;

public class EquipmentCapacityTests
{
    private static DateTimeOffset At(int hour) => new(2026, 11, 10, hour, 0, 0, TimeSpan.Zero);
    [Fact]
    public void SequentialReservationsAreNotAddedTogether()
    {
        var spans = new[] { new QuantitySpan(At(8), At(10), 4), new QuantitySpan(At(10), At(12), 4) };
        Assert.Equal(4, EquipmentCapacity.Peak(spans, At(8), At(12)));
    }
    [Fact]
    public void OverlapsAndClippedBoundariesUsePeak()
    {
        var spans = new[] { new QuantitySpan(At(6), At(9), 3), new QuantitySpan(At(8), At(10), 2), new QuantitySpan(At(10), At(12), 7) };
        Assert.Equal(5, EquipmentCapacity.Peak(spans, At(8), At(10)));
        Assert.Equal(7, EquipmentCapacity.Peak(spans, At(9), At(11)));
    }
}
