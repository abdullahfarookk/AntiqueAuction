﻿#nullable enable
using System;
using System.Collections.Generic;
using AntiqueAuction.Shared.Domain;
using AntiqueAuction.Shared.Exceptions;
using AntiqueAuction.Shared.Threading;

namespace AntiqueAuction.Core.Models
{
    public class User:AggregateRoot
    {
        public string Name { get; protected set; } = null!;
        public string Username { get; protected set; } = null!;
        public string PasswordHash { get; protected set; } = null!;
        public string? ContactNo { get; protected set; }
        public double AvailableAmount { get; protected set; }


        public virtual Address Address { get; protected set; } = null!;
        public virtual ICollection<AutoBid> AutoBids { get; set; }
        public virtual ICollection<BidHistory> ItemHistories { get; set; }
        private readonly MultiObjectLocker<Guid> _locker = new MultiObjectLocker<Guid>();

        protected User(){}
        public User(string name,string username,string passwordHash,double availableAmount,Address address)
        {
            Name = name;
            Username = username;
            PasswordHash = passwordHash;
            AvailableAmount = availableAmount;
            Address = address;
        }

        public void DrawMoney(double money)
        {
            // enter a new locker object for each UserId if it's not already in dictionary
            lock (_locker.Enter(Id))
            {
                if (AvailableAmount - money < 0)
                    throw new UnprocessableException("Cannot Draw money which is greater than available money");
                AvailableAmount -= money;
                _locker.Release(Id);
            }
            
        }

    }
}