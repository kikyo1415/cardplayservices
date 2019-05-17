using System;

namespace PokerCardPlay.Contract
{
    public interface IGameHallContract
    {
        string CreateGameRoom();
        bool ExitGameRoom(string roomNumber);
        string GetPlayerNowRoomNum(Guid playerId);
        bool JoinGameRoom(string roomNumber);
    }

}
