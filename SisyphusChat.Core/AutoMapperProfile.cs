using AutoMapper;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;


namespace Hermes.Core;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.UserName))
            .ForMember(x => x.LastUpdated, opt => opt.MapFrom(x => x.LastUpdated))
            .ForMember(x => x.IsOnline, opt => opt.MapFrom(x => x.IsOnline))
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.UserName))
            .ForMember(x => x.ChatUsers, opt => opt.MapFrom(x => x.ChatUsers))
            .ReverseMap();

        CreateMap<Chat, ChatModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(x => x.OwnerId, opt => opt.MapFrom(x => x.Owner))
            .ForMember(x => x.ChatUsers, opt => opt.MapFrom(x => x.ChatUsers))
            .ForMember(x => x.Messages, opt => opt.MapFrom(x => x.Messages))
            .ForMember(x => x.Type, opt => opt.MapFrom(x => x.Type))
            .ReverseMap();

        CreateMap<Message, MessageModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.ChatId, opt => opt.MapFrom(x => x.ChatId))
            .ForMember(x => x.Chat, opt => opt.MapFrom(x => x.Chat))
            .ForMember(x => x.SenderId, opt => opt.MapFrom(x => x.SenderId))
            .ForMember(x => x.Content, opt => opt.MapFrom(x => x.Content))
            .ForMember(x => x.LastUpdated, opt => opt.MapFrom(x => x.LastUpdated))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.Status))
            .ReverseMap();

        CreateMap<ChatUser, ChatUserModel>()
            .ForMember(x => x.ChatId, opt => opt.MapFrom(x => x.ChatId))
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.UserId))
            .ForMember(x => x.Chat, opt => opt.MapFrom(x => x.Chat))
            .ForMember(x => x.User, opt => opt.MapFrom(x => x.User))
            .ReverseMap();
    }
}
