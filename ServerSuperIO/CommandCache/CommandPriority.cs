using ServerSuperIO.Common;

namespace ServerSuperIO.CommandCache
{
    public enum CommandPriority
    {
        [EnumDescription("正常发送")]
        Normal = 0x00,
        [EnumDescription("优先发送")]
        High
    }
}
