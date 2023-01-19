using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnergyMonitor.Data.Entity;

[Table("usage")]
public class UsageReading
{
    public long id { get; set; }
    public DateTime dateTimeUTC { get; set; }
    public int device { get; set; }
    public ushort resetCount { get; set; }
    public decimal volts { get; set; }
    public uint seconds { get; set; }
    public decimal ch1Current { get; set; }
    public decimal ch2Current { get; set; }
    public bool ch1Polarity { get; set; }
    public bool ch2Polarity { get; set; }
    public ulong ch1AbsWS { get; set; }
    public ulong ch2AbsWS { get; set; }
    public ulong ch1PolWS { get; set; }
    public ulong ch2PolWS { get; set; }
    public uint aux1WS { get; set; }
    public uint aux2WS { get; set; }
    public uint aux3WS { get; set; }
    public uint aux4WS { get; set; }
    public uint aux5WS { get; set; }
    public int ch1Watts { get; set; }
    public int ch2Watts { get; set; }
    public int aux1Watts { get; set; }
    public int aux2Watts { get; set; }
    public int aux3Watts { get; set; }
    public int aux4Watts { get; set; }
    public decimal aux5Watts { get; set; }
    public int otherLoads { get; set; }
}


public interface IHaveNullableStartAndEndDates
{
    DateTime? StartDate { get; }
    DateTime? EndDate { get; }
}

[Table("brultech_device")]
[PrimaryKey(nameof(DeviceId))]
public class BrultechDevice
{
    [Required]
    public int DeviceId { get; set; }
    [MaxLength(45)]
    public string Description { get; set; }
    public byte CommandByte { get; set; }


    // Navigators
    public List<BrultechSourceChannelAssignment> SourceChannelAssignments { get; set; }
    public List<BrultechLoadChannelAssignment> LoadChannelAssignments { get; set; }
}


[Table("brultech_channel")]
[PrimaryKey(nameof(ChannelId))]
public class BrultechChannel
{
    public int ChannelId { get; set; }
    [MaxLength(45)]
    public string Name { get; set; }

}

[Table("brultech_source")]
[PrimaryKey(nameof(SourceId))]
public class BrultechSource
{
    [Required]
    public long SourceId { get; set; }

    [Required]
    public string Name { get; set; }


    // Navigators
    public List<BrultechSourceChannelAssignment> ChannelAssignments { get; set; }
}

[Table("brultech_load")]
[PrimaryKey(nameof(LoadId))]
public class BrultechLoad
{
    [Required]
    public long LoadId { get; set; }

    [Required]
    public string Name { get; set; }
}


[Table("brultech_source_assignment")]
[PrimaryKey(nameof(SourceChannelAssignmentId))]
public class BrultechSourceChannelAssignment : IHaveNullableStartAndEndDates
{
    [Required]
    public long SourceChannelAssignmentId { get; set; }

    [Required]
    [ForeignKey(nameof(SourceId))]
    public long SourceId { get; set; }

    [Required]
    [ForeignKey(nameof(DeviceId))]
    public int DeviceId { get; set; }

    [Required]
    [ForeignKey(nameof(ChannelId))]
    public int ChannelId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigators
    public BrultechDevice Device { get; set; }
    public BrultechSource Source { get; set; }
}


[Table("brultech_load_assignment")]
[PrimaryKey(nameof(LoadChannelAssignmentId))]
public class BrultechLoadChannelAssignment : IHaveNullableStartAndEndDates
{
    [Required]
    public long LoadChannelAssignmentId { get; set; }

    [Required]
    [ForeignKey(nameof(LoadId))]
    public long LoadId { get; set; }

    [Required]
    [ForeignKey(nameof(SourceId))]
    public long SourceId { get; set; }

    [Required]
    [ForeignKey(nameof(DeviceId))]
    public int DeviceId { get; set; }

    [Required]
    [ForeignKey(nameof(ChannelId))]
    public int ChannelId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }



    // Navigators
    public BrultechDevice Device { get; set; }
    public BrultechSource Source { get; set; }
    public BrultechLoad Load { get; set; }
}