﻿using System.ComponentModel.DataAnnotations;

namespace WebFrontend.Models
{
    public class BookingViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string StartLocation { get; set; }
        [Required]
        public string EndLocation { get; set; }
        [Required]
        public DateTime RideDateTime {  get; set; }
        [Required]
        public int PassengerCount {  get; set; }
        [Required]
        public string CabType {  get; set; }
        [Required]
        public float BaseFarePrice {  get; set; }
        [Required]
        public float TotalPrice {  get; set; }
    }
}