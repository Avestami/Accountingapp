using AutoMapper;
using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Mappings
{
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            CreateMap<Voucher, VoucherDto>()
                .ForMember(dest => dest.Entries, opt => opt.MapFrom(src => src.Entries));

            CreateMap<VoucherEntry, VoucherEntryDto>()
                .ForMember(dest => dest.DebitAmount, opt => opt.MapFrom(src => src.TransactionType == TransactionType.Debit ? src.Amount : 0))
                .ForMember(dest => dest.CreditAmount, opt => opt.MapFrom(src => src.TransactionType == TransactionType.Credit ? src.Amount : 0));

            CreateMap<Account, AccountDto>();
        }
    }
}