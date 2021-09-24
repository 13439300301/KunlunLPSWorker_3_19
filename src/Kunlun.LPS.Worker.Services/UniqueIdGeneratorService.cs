using IdGen;
using Microsoft.Extensions.Configuration;
using System;

namespace Kunlun.LPS.Worker.Services
{
    public class UniqueIdGeneratorService : IUniqueIdGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly IdGenerator _generator;

        public UniqueIdGeneratorService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Max.generators        : 512
            // Id's/ms per generator : 8192
            // Id's/ms total         : 4194304
            // Wraparound interval   : 25451.15:47:35.5520000
            // Wraparound date       : 2089-07-07T15:47:35.5520000+00:00

            var mc = new MaskConfig(41, 9, 13);
            _generator = new IdGenerator(GetMachineCode(), new DateTime(2019, 11, 1, 0, 0, 0, DateTimeKind.Utc), mc);

        }

        public long Next()
        {
            return _generator.CreateId();
        }

        private int GetMachineCode()
        {
            return _configuration.GetValue<int>("MachineCode");
        }
    }

}
