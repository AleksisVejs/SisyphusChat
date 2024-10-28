using AutoMapper;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline))
                .ForMember(dest => dest.Chats, opt => opt.MapFrom(src => src.Chats))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Friends, opt => opt.MapFrom(src => src.Friends))
                .ReverseMap();

            CreateMap<Chat, ChatModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id)) // Mapping owner entity to OwnerId
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner)) // Mapping owner entity to UserModel
                .ForMember(dest => dest.ChatUsers, opt => opt.MapFrom(src => src.ChatUsers))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ReverseMap();

            CreateMap<Message, MessageModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom(src => src.Chat))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();

            CreateMap<ChatUser, ChatUserModel>()
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom(src => src.Chat))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();

            CreateMap<Friend, FriendModel>()
                .ForMember(dest => dest.ReqSenderId, opt => opt.MapFrom((src) => src.ReqSenderId))
                .ForMember(dest => dest.ReqSender, opt => opt.MapFrom(src => src.ReqSender))
                .ForMember(dest => dest.ReqReceiverId, opt => opt.MapFrom(src => src.ReqReceiverId))
                .ForMember(dest => dest.ReqReceiver, opt => opt.MapFrom(src => src.ReqReceiver))
                .ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(src => src.IsAccepted))
                .ReverseMap();

            CreateMap<Notification, NotificationModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.SenderUserId, opt => opt.MapFrom(src => src.SenderUsername))
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.SenderUsername))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.TimeCreated))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.RelatedEntityId))
                .ForMember(dest => dest.RelatedEntityType, opt => opt.MapFrom(src => src.RelatedEntityType))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
        }
    }
}
