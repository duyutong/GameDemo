using Network.Models;
using System;
using UnityEngine;
namespace Network.API
{
    public class RoomApi : HttpMessageApi
    {
        #region AutoContext
        public async void RoomCreate(RoomCreateRequest req, Action<bool, RoomCreateResponse> action)
        {
            await PostAsync<RoomCreateRequest, RoomCreateResponse>("/room/create", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else
                {
                    Debug.LogError($"RoomApi RoomCreate failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null);
                }
            });
        }
        public async void RoomGetRooms(RoomGetRoomsRequest req, Action<bool, RoomGetRoomsResponse> action)
        {
            await PostAsync<RoomGetRoomsRequest, RoomGetRoomsResponse>("/room/getRooms", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else
                {
                    Debug.LogError($"RoomApi RoomGetRooms failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null);
                }
            });
        }
        public async void RoomFindRoom(RoomFindRoomRequest req, Action<bool, RoomFindRoomResponse> action)
        {
            await PostAsync<RoomFindRoomRequest, RoomFindRoomResponse>("/room/findRoom", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else
                {
                    Debug.LogError($"RoomApi RoomFindRoom failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null);
                }
            });
        }
        #endregion HttpFuncStr
    }
}