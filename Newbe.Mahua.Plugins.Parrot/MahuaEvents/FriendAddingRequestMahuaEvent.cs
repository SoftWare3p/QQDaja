using Newbe.Mahua.MahuaEvents;
using System;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 好友申请接受事件
    /// </summary>
    public class FriendAddingRequestMahuaEvent
        : IFriendAddingRequestMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public FriendAddingRequestMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessAddingFriendRequest(FriendAddingRequestContext context)
        {
            // todo 填充处理逻辑
            _mahuaApi.AcceptFriendAddingRequest(context.AddingFriendRequestId,context.FromQq,"QQ："+ context.FromQq);
            
        }
    }
}
