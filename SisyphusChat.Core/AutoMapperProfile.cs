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
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                    src.IsDeleted ? "Deleted User" : src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => 
                    src.IsDeleted ? "deleted@deleted.com" : src.Email))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.IsBanned))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => 
                    src.IsDeleted ? false : src.IsOnline))
                .ForMember(dest => dest.TimeCreated, opt => opt.MapFrom(src => src.TimeCreated))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => 
                    src.IsDeleted ? null : src.Picture))
                .ForMember(dest => dest.IsProfanityEnabled, opt => opt.MapFrom(src => src.IsProfanityEnabled))
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
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => 
                    src.Sender.IsDeleted ? "DELETED_USER" : src.SenderId))
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

            CreateMap<Report, ReportModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom((src) => src.Id))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom((src) => src.ChatId))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom((src) => src.Chat))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom((src) => src.MessageId))
                .ForMember(dest => dest.Message, opt => opt.MapFrom((src) => src.Message))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src) => src.Type))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom((src) => src.Reason))
                .ForMember(dest => dest.TimeCreated, opt => opt.MapFrom((src) => src.TimeCreated))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom((src) => src.LastUpdated))
                .ReverseMap();

            CreateMap<Notification, NotificationModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.SenderUserId, opt => opt.MapFrom(src => src.SenderUsername))
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.SenderUsername))
                .ForMember(dest => dest.TimeCreated, opt => opt.MapFrom(src => src.TimeCreated))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.RelatedEntityId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ReverseMap();

        }
    }
}