﻿using Domain;
using Newtonsoft.Json;
using System;

namespace Application.Models
{
    public class UserAdvertiseDTO
    {
       
        [JsonProperty("userAdvertise")]
        public Root Root { get; set; }
        public UserAdvertiseDTO()
        {

        }
        public UserAdvertiseDTO(UserAdvertise userAdvertise)
        {
            
            Root = new Root
            {
               
                IsNegotiate = userAdvertise.IsNegotiate,
                IsOnWarranty = userAdvertise.IsOnWarranty,
                PaymentOption = userAdvertise.PaymentOption,
                Status = userAdvertise.Status,
                AdvertiseDTO = new AdvertiseDTO
                { Id = userAdvertise.Advertise.Id,
                UniqueId = userAdvertise.Advertise.UniqueId,
                    Title = userAdvertise.Advertise.Title,
                    District = userAdvertise.Advertise.District,
                    City = userAdvertise.Advertise.City,
                    Price = userAdvertise.Advertise.Price,
                    PublishedAt = userAdvertise.Advertise.PublishedAt,
                    AdvertiseInfoDTO = new AdvertiseInfoDTO
                    {
                        Quantity = userAdvertise.Advertise.AdvertiseInfo.Quantity,
                        Hint = userAdvertise.Advertise.AdvertiseInfo.Hint,
                        Color = userAdvertise.Advertise.AdvertiseInfo.Color,
                        Description = userAdvertise.Advertise.AdvertiseInfo.Description
                    }
                }
            };

  
        }

    }
    public class AdvertiseDTO
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }
        public string Title { get;  set; }
        public string District { get;  set; }
        public string City { get;  set; }
        public double Price { get;  set; }
        public DateTime PublishedAt { get;  set; }
        [JsonProperty("advertiseInfo")]
        public AdvertiseInfoDTO AdvertiseInfoDTO { get; set; }

    }
    public class AdvertiseInfoDTO
    {
        public string Color { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public byte Quantity { get; set; }
    }
    public class Root
    {
    
        public string Category{get;set;}
        public bool IsNegotiate { get; set; }
        public bool IsOnWarranty { get; set; }
        public Status Status { get; set; }
        public PaymentOption PaymentOption { get; set; }
        [JsonProperty("advertise")]
        public AdvertiseDTO AdvertiseDTO { get; set; }
         [JsonProperty("user")]
        public AdvertiseUser User { get; set; }

    }
      public class AdvertiseUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}