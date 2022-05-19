using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class FileDataAccess : IDataAccess
    {
        public async Task<SaveState> LoadAsync(string filename)
        {
            using (var sReader = new StreamReader(filename))
            {
                return SaveState.Parse(await sReader.ReadToEndAsync());
            }
        }

        public async Task SaveAsync(string filename, SaveState state)
        {
            using (var sWriter = new StreamWriter(filename))
            {
                await sWriter.WriteAsync(state.ToFileString());
            }
        }
    }
}
