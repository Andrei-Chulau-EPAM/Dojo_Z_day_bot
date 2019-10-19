using System.Collections.Generic;
using CodeBattleNet.Core;

namespace CodeBattleNet.Utilites
{
    class CommandsProcessor
    {
        private readonly BattleNetClient _client;

        private readonly Dictionary<Movement, string> _movementMap;

        public CommandsProcessor(BattleNetClient client)
        {
            _client = client;

            _movementMap = new Dictionary<Movement, string>
            {
                {Movement.Down, _client.Down() },
                {Movement.Up, _client.Up() },
                {Movement.Left, _client.Left() },
                {Movement.Right, _client.Right() },
                {Movement.Stop, _client.Blank() },
            };
        }


        public string CreateCommand(bool preAct, Movement movement, bool postAct)
        {
            if (movement == Movement.Stop)
            {
                if (preAct || postAct)
                {
                    return _client.Act();
                }
                else
                {
                    return _movementMap[Movement.Stop];
                }
            }

            if (preAct && postAct)
            {
                postAct = false;
            }

            var preActCommand = preAct ? _client.Act() + "," : string.Empty;
            var movementCommand = _movementMap[movement];
            var postActCommand = postAct ? "," + _client.Act() : string.Empty;

            return preActCommand + movementCommand + postActCommand;
        }
    }
}
