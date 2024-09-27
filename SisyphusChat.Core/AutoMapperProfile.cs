/*using AutoMapper;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hermes.Core;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.UserName))
            .ForMember(x => x.LastLoginAt, opt => opt.MapFrom(x => x.LastLoginAt))
            .ForMember(x => x.IsOnline, opt => opt.MapFrom(x => x.IsOnline))
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.UserName))
            .ForMember(x => x.Chats, opt => opt.MapFrom(x => x.Chats))
            .ReverseMap();

        CreateMap<Chat, ChatModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(x => x.Owner, opt => opt.MapFrom(x => x.Owner))
            .ForMember(x => x.Members, opt => opt.MapFrom(x => x.Members))
            .ForMember(x => x.Messages, opt => opt.MapFrom(x => x.Messages))
            .ForMember(x => x.Type, opt => opt.MapFrom(x => x.Type))
            .ReverseMap();

        CreateMap<Message, MessageModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.ChatId, opt => opt.MapFrom(x => x.ChatId))
            .ForMember(x => x.Chat, opt => opt.MapFrom(x => x.Chat))
            .ForMember(x => x.Sender, opt => opt.MapFrom(x => x.Sender))
            .ForMember(x => x.SenderId, opt => opt.MapFrom(x => x.SenderId))
            .ForMember(x => x.Content, opt => opt.MapFrom(x => x.Content))
            .ForMember(x => x.SentAt, opt => opt.MapFrom(x => x.SentAt))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.Status))
            .ReverseMap();

        CreateMap<ChatOwner, ChatOwnerModel>()
            .ForMember(x => x.Chat, opt => opt.MapFrom(x => x.Chat))
            .ForMember(x => x.ChatId, opt => opt.MapFrom(x => x.ChatId))
            .ForMember(x => x.Owner, opt => opt.MapFrom(x => x.Owner))
            .ForMember(x => x.OwnerId, opt => opt.MapFrom(x => x.OwnerId))
            .ReverseMap();

        CreateMap<ChatUser, ChatUserModel>()
            .ForMember(x => x.Chat, opt => opt.MapFrom(x => x.Chat))
            .ForMember(x => x.ChatId, opt => opt.MapFrom(x => x.ChatId))
            .ForMember(x => x.User, opt => opt.MapFrom(x => x.User))
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.UserId))
            .ReverseMap();
    }
}
*/