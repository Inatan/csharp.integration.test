namespace Alura.CoisasAFazer.Services.Handlers
{
    public class CommandResult
    {
        public CommandResult(bool isSucesss)
        {
            this.isSucesss = isSucesss;
        }

        public bool isSucesss { get; set; }
    }
}
