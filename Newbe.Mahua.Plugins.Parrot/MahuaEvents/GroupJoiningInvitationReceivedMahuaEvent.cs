using Newbe.Mahua.MahuaEvents;
using System;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 入群邀请接收事件
    /// </summary>
    public class GroupJoiningInvitationReceivedMahuaEvent
        : IGroupJoiningInvitationReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupJoiningInvitationReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessJoinGroupRequest(GroupJoiningRequestReceivedContext context)
        {
            // todo 填充处理逻辑
            _mahuaApi.AcceptGroupJoiningInvitation(context.GroupJoiningRequestId,context.ToGroup,context.FromQq);
        }
        public void ProcessJoinGroupInvitation(GroupJoiningInvitationReceivedContext context)
        {
            _mahuaApi.AcceptGroupJoiningInvitation(context.GroupJoiningInvitationId, context.ToGroup, context.FromQq);
        }
    }
}
