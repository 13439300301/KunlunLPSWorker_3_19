using AutoMapper;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.MessageQueue.AutoMapper
{
    public  class AutoMapperConfiguration
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;

        public  void Init()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<TopupMessage, TopupMessageShown>();
            }
            );
            _mapper = _mapperConfiguration.CreateMapper();
        }

        public static IMapper Mapper => _mapper;

        public static MapperConfiguration MapperConfiguration => _mapperConfiguration;

        
    }
}
