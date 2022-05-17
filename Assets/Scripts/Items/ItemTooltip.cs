using System.Text;

namespace ResourceRun.Items
{
    public class ItemTooltip
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void Add(string line)
        {
            _builder.Append($"{line}\n");
        }

        public string Get()
        {
            return _builder.ToString();
        }
    }
}