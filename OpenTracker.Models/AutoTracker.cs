﻿using OpenTracker.Models.AutotrackerConnectors;
using OpenTracker.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using WebSocketSharp;

namespace OpenTracker.Models
{
    public class AutoTracker : INotifyPropertyChanging, INotifyPropertyChanged
    {

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        private USB2SNESConnector _connector;
        public USB2SNESConnector Connector
        {
            get => _connector;
            set
            {
                if (_connector != value)
                {
                    OnPropertyChanging(nameof(Connector));
                    _connector = value;
                    OnPropertyChanged(nameof(Connector));
                }
            }
        }

        private byte? _inGameStatus;
        public byte? InGameStatus
        {
            get => _inGameStatus;
            private set
            {
                if (_inGameStatus != value)
                {
                    _inGameStatus = value;
                    OnPropertyChanged(nameof(InGameStatus));
                }
            }
        }

        public List<MemoryAddress> RoomMemory { get; }
        public List<MemoryAddress> OverworldEventMemory { get; }
        public List<MemoryAddress> ItemMemory { get; }
        public List<MemoryAddress> NPCItemMemory { get; }

        public Action<string, LogLevel> MessageHandler { get; set; }

        public AutoTracker()
        {
            RoomMemory = new List<MemoryAddress>(592);
            OverworldEventMemory = new List<MemoryAddress>(130);
            ItemMemory = new List<MemoryAddress>(144);
            NPCItemMemory = new List<MemoryAddress>(2);

            for (int i = 0; i < 592; i++)
            {
                RoomMemory.Add(new MemoryAddress());

                if (i < 130)
                    OverworldEventMemory.Add(new MemoryAddress());

                if (i < 144)
                    ItemMemory.Add(new MemoryAddress());

                if (i < 2)
                    NPCItemMemory.Add(new MemoryAddress());
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(InGameStatus))
                return;
        }

        public void Start(string uriString)
        {
            Connector = new USB2SNESConnector(uriString, MessageHandler);

            Connector.ConnectIfNecessary();

            int i = 0;

            while (Connector != null && !Connector.Connected && i <= 3)
            {
                Thread.Sleep(1000);
                i++;
            }
        }

        public void Stop()
        {
            if (Connector != null)
            {
                Connector.Dispose();
                Connector = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            InGameStatus = null;

            foreach (MemoryAddress address in RoomMemory)
                address.Reset();

            foreach (MemoryAddress address in OverworldEventMemory)
                address.Reset();

            foreach (MemoryAddress address in ItemMemory)
                address.Reset();

            foreach (MemoryAddress address in NPCItemMemory)
                address.Reset();
        }

        public bool IsInGame()
        {
            if (InGameStatus.HasValue && InGameStatus.Value > 0x05 &&
                InGameStatus.Value != 0x14)
                return true;

            return false;
        }

        public void InGameCheck()
        {
            if (Connector != null && Connector.Connected)
            {
                try
                {
                    _connector.ReadByte(0x7e0010, out byte inGameStatus);
                    InGameStatus = inGameStatus;
                }
                catch (Exception ex)
                {
                    MessageHandler?.Invoke(ex.Message, LogLevel.Warn);
                }
            }
        }

        public void MemoryCheck()
        {
            if (Connector != null && Connector.Connected)
            {
                try
                {
                    if (IsInGame())
                    {
                        byte[] buffer = new byte[592];
                        _connector.Read(0x7ef000, buffer);

                        for (int i = 0; i < buffer.Length; i++)
                            RoomMemory[i].Value = buffer[i];

                        buffer = new byte[130];
                        _connector.Read(0x7ef280, buffer);

                        for (int i = 0; i < buffer.Length; i++)
                            OverworldEventMemory[i].Value = buffer[i];

                        buffer = new byte[144];
                        _connector.Read(0x7ef340, buffer);

                        for (int i = 0; i < buffer.Length; i++)
                            ItemMemory[i].Value = buffer[i];

                        buffer = new byte[2];
                        _connector.Read(0x7ef410, buffer);

                        for (int i = 0; i < buffer.Length; i++)
                            NPCItemMemory[i].Value = buffer[i];
                    }
                }
                catch (Exception ex)
                {
                    MessageHandler?.Invoke(ex.Message, LogLevel.Warn);
                }
            }
        }

        public bool? CheckMemoryFlag(MemorySegmentType segment, int index, byte flag)
        {
            List<MemoryAddress> memory = null;

            switch (segment)
            {
                case MemorySegmentType.Room:
                    memory = RoomMemory;
                    break;
                case MemorySegmentType.OverworldEvent:
                    memory = OverworldEventMemory;
                    break;
                case MemorySegmentType.Item:
                    memory = ItemMemory;
                    break;
                case MemorySegmentType.NPCItem:
                    memory = NPCItemMemory;
                    break;
            }

            if (memory == null)
                throw new ArgumentOutOfRangeException(nameof(segment));

            if (memory.Count > index)
            {
                if ((memory[index].Value & flag) != 0)
                    return true;
                else
                    return false;
            }

            return null;
        }

        public int? CheckMemoryFlagArray((MemorySegmentType, int, byte)[] addressFlags)
        {
            if (addressFlags == null)
                throw new ArgumentNullException(nameof(addressFlags));
            
            int? result = null;

            foreach ((MemorySegmentType, int, byte) address in addressFlags)
            {
                bool? addressResult = CheckMemoryFlag(address.Item1, address.Item2, address.Item3);

                if (addressResult.HasValue)
                {
                    if (addressResult.Value)
                    {
                        if (result.HasValue)
                            result++;
                        else
                            result = 1;
                    }
                    else if (!result.HasValue)
                        result = 0;
                }
            }

            return result;
        }

        public bool? CheckMemoryByte(MemorySegmentType segment, int index, byte minValue = 0)
        {
            List<MemoryAddress> memory = null;

            switch (segment)
            {
                case MemorySegmentType.Room:
                    memory = RoomMemory;
                    break;
                case MemorySegmentType.OverworldEvent:
                    memory = OverworldEventMemory;
                    break;
                case MemorySegmentType.Item:
                    memory = ItemMemory;
                    break;
                case MemorySegmentType.NPCItem:
                    memory = NPCItemMemory;
                    break;
            }

            if (memory == null)
                throw new ArgumentOutOfRangeException(nameof(segment));

            if (memory.Count > index)
            {
                if (memory[index].Value > minValue)
                    return true;
                else
                    return false;
            }

            return null;
        }

        public byte? CheckMemoryByteValue(MemorySegmentType segment, int index)
        {
            List<MemoryAddress> memory = null;

            switch (segment)
            {
                case MemorySegmentType.Room:
                    memory = RoomMemory;
                    break;
                case MemorySegmentType.OverworldEvent:
                    memory = OverworldEventMemory;
                    break;
                case MemorySegmentType.Item:
                    memory = ItemMemory;
                    break;
                case MemorySegmentType.NPCItem:
                    memory = NPCItemMemory;
                    break;
            }

            if (memory == null)
                throw new ArgumentOutOfRangeException(nameof(segment));

            if (memory.Count > index)
                return memory[index].Value;

            return null;
        }

        public int? CheckMemoryByteArray((MemorySegmentType, int)[] addresses)
        {
            if (addresses == null)
                throw new ArgumentNullException(nameof(addresses));

            int? result = null;

            foreach ((MemorySegmentType, int) address in addresses)
            {
                bool? addressResult = CheckMemoryByte(address.Item1, address.Item2);

                if (addressResult.HasValue)
                {
                    if (addressResult.Value)
                    {
                        if (result.HasValue)
                            result++;
                        else
                            result = 1;
                    }
                    else if (!result.HasValue)
                        result = 0;
                }
            }

            return result;
        }
    }
}