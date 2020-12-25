﻿using LSS.BE.Core.Domain.Services;
using LSS.BE.Core.Entities.Models;
using LSS.BE.Core.TestCaller.Models;
using LSS.HCM.Core.DataObjects.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSS.BE.Core.TestCaller
{
    public class GetewayServiceClient
    {
        public static (string, LmsGatewayService) Init()
        {
            Console.Write("Gateway Service Initialization\n");

            Console.Write("Uri String: http://18.138.61.187\n");
            string uriString = "http://18.138.61.187";

            Console.Write("LockerStation Id: c17fb923-70f9-4d3c-b081-4226096d6905\n");
            string lockerStationId = "c17fb923-70f9-4d3c-b081-4226096d6905";
            Console.Write("Version: v1\n");
            string version = "v1";

            Console.Write("Client Id: ef3350f9-ace2-4900-9da0-bba80402535a\n");
            string clientId = "ef3350f9-ace2-4900-9da0-bba80402535a";

            Console.Write("Client Secret: FA1s0QmZFxXh44QUkVOcEj19hvhjWTsfl1sslwGO\n");
            string clientSecret = "FA1s0QmZFxXh44QUkVOcEj19hvhjWTsfl1sslwGO";

            Console.Write("Configuration Path : ");
            string configurationPath = Console.ReadLine();

            var gatewayService = new LmsGatewayService(uriString, version, clientId, clientSecret, configurationPath);
            Console.WriteLine("Gateway Service Initialized");

            return (lockerStationId, gatewayService);
        }

        public static void CourierDropOff(string lockerStationId, LmsGatewayService gatewayService)
        {

            #region Lsp Verification

            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write("[Lsp Verification][Req]\n");

            Console.Write("Key: ");
            string key = Console.ReadLine();

            Console.Write("Pin: ");
            string pin = Console.ReadLine();


            var lspVerification = new LspUserAccess
            {
                LockerStationid = lockerStationId,
                Key = key,
                Pin = pin
            };
            var lspVerificationResult = gatewayService.LspVerification(lspVerification);

            Console.WriteLine("[Lsp Verification][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(lspVerificationResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            var lspVerificationResponse = JsonConvert.DeserializeObject<LspVerificationResponse>(lspVerificationResult.ToString());

            #endregion

            #region Verify Otp

            Console.WriteLine("[Verify Otp][Req]");

            Console.Write("Code: ");
            string code = Console.ReadLine();

            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine();


            var verifyOtpModel = new VerifyOtp
            {
                LockerStationId = lockerStationId,
                LspId = lspVerificationResponse.LspId,
                Code = code,
                PhoneNumber = phoneNumber,
                RefCode = lspVerificationResponse.RefCode
            };

            var verifyOtpResult = gatewayService.VerifyOtp(verifyOtpModel);

            Console.WriteLine("[Verify Otp][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(verifyOtpResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Receive Locker Station Details
            Console.WriteLine("[Receive Locker Station Details][Req]");
            var lockerStationDetailsResult = gatewayService.LockerStationDetails(lockerStationId);

            Console.WriteLine("[Receive Locker Station Details][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(lockerStationDetailsResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            var lockerStationDetailsResponse = JsonConvert.DeserializeObject<LspVerificationResponse>(lspVerificationResult.ToString());

            #endregion

            #region Find Booking By Tracking Number

            Console.WriteLine("[Find Booking By Tracking Number][Req]");

            Console.Write("Tracking Number: ");
            string trackingNumber = Console.ReadLine();

            if (lspVerificationResponse.LspId == null) lspVerificationResponse.LspId = string.Empty;
            var findBookingResult = gatewayService.FindBooking(trackingNumber, lockerStationId, lspVerificationResponse.LspId);

            Console.WriteLine("[Find Booking By Tracking Number][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(findBookingResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Assign Similar Size Locker
            Console.WriteLine("[Assign Similar Size Locker][Req]");

            Console.Write("Booking Id: ");
            string bookingId = Console.ReadLine();

            Console.Write("Reason: ");
            string reason = Console.ReadLine();

            var assignSimilarSizeLocker = new AssignSimilarSizeLocker()
            {
                LockerStationId = lockerStationId,
                BookingId = Convert.ToInt32(bookingId),
                Reason = reason
            };

            var similarSizeLockerResult = gatewayService.AssignSimilarSizeLocker(assignSimilarSizeLocker);
            Console.WriteLine("[Assign Similar Size Locker][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(similarSizeLockerResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Get Available Sizes
            Console.WriteLine("[Get Available Sizes][Req]");
            Console.Write("Locker Station Id: c17fb923-70f9-4d3c-b081-4226096d6905\n");

            var getAvailableSizesResult = gatewayService.GetAvailableSizes(lockerStationId);
            Console.WriteLine("[Get Available Sizes][Res]");

            Console.WriteLine(JsonConvert.SerializeObject(getAvailableSizesResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion

            #region Change Locker Size
            Console.WriteLine("[Change Locker Size][Req]");

            Console.Write("Booking Id: ");
            string changeLockerSizeBookingId = Console.ReadLine();

            Console.Write("Size: ");
            string size = Console.ReadLine();

            var changeLockerSize = new ChangeLockerSize()
            {
                LockerStationId = lockerStationId,
                BookingId = changeLockerSizeBookingId,
                Size = size
            };

            var changeLockerSizeResult = gatewayService.ChangeLockerSize(changeLockerSize);
            Console.WriteLine("[Change Locker Size][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(changeLockerSizeResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");



            #endregion

            #region Open Compartment

            Console.WriteLine("[Open Compartment][Req]");
            string transactionId = Guid.NewGuid().ToString();

            Console.Write("Locker Id: ");
            string lockerId = Console.ReadLine();

            Console.Write("Compartment Id: ");
            string compartmentIds = Console.ReadLine();
            string[] compartmentId = compartmentIds.Split(',');

            var openCompartment = new HCM.Core.DataObjects.Models.Compartment(transactionId, lockerId, compartmentId, false, string.Empty, string.Empty);
            var openCompartmentResult = gatewayService.OpenCompartment(openCompartment);

            Console.WriteLine("[Open Compartment][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(openCompartmentResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Compartment Status

            Console.WriteLine("[Compartment Status][Req]");

            Console.Write("Locker Id: ");
            lockerId = Console.ReadLine();

            Console.Write("Compartment Id: ");
            compartmentIds = Console.ReadLine();
            compartmentId = compartmentIds.Split(',');

            var compartmentStatus = new HCM.Core.DataObjects.Models.Compartment(transactionId, lockerId, compartmentId, false, string.Empty, string.Empty);
            var compartmentStatusResult = gatewayService.CompartmentStatus(compartmentStatus);

            Console.WriteLine("[Compartment Status][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(compartmentStatusResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion

            #region Capture Image

            Console.WriteLine("[Capture Image][Req]");

            Console.Write("Locker Id: ");
            lockerId = Console.ReadLine();

            var captureImage = new Capture(transactionId, lockerId, false, string.Empty, string.Empty);
            var captureImageResult = gatewayService.CaptureImage(captureImage);

            Console.WriteLine("[Capture Image][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(captureImageResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");


            #endregion

            #region Update Booking Status

            Console.WriteLine("[Update Booking Status][Req]");

            Console.Write("Booking Id: ");
            string UpdateBookingStatusBookingId = Console.ReadLine();

            Console.Write("Status: ");
            string status = Console.ReadLine();

            Console.Write("MobileNumber: ");
            string mobileNumber = Console.ReadLine();

            Console.Write("Reason: ");
            string updateBookingReason = Console.ReadLine();

            var bookingStatusUpdate = new BookingStatus()
            {
                LockerStationId = lockerStationId,
                BookingId = Convert.ToInt32(UpdateBookingStatusBookingId),
                LspId = lspVerificationResponse.LspId,
                MobileNumber = mobileNumber,
                Status = status,
                Reason = updateBookingReason

            };

            var bookingStatusUpdateResult = gatewayService.UpdateBookingStatus(bookingStatusUpdate);
            Console.WriteLine("[Update Booking Status][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(bookingStatusUpdateResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");


            #endregion
        }

        public static void ConsumerCollect(string lockerStationId, LmsGatewayService gatewayService) 
        {
            #region PIN Verification

            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write("[Consumer Collect PIN Verification][Req]\n");

            Console.Write("Pin: ");
            string ConsumerCollectPin = Console.ReadLine();

            Console.Write("Action: ");
            string action = Console.ReadLine();


            var consumerPin = new ConsumerPin
            {
                LockerStationId = lockerStationId,
                Pin = ConsumerCollectPin,
                Action = action
            };
            var consumerPinResult = gatewayService.GetBookingByConsumerPin(consumerPin);

            Console.WriteLine("[Consumer Collect PIN Verification][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(consumerPinResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Send OTP
            Console.WriteLine("[Send Otp][Req]\n");

            Console.Write("MobileNumber: ");
            string consumerCollectMobileNumber = Console.ReadLine();

            Console.Write("BookingId: ");
            string consumerBookingId = Console.ReadLine();

            Console.Write("LspId: ");
            string consumerLspId = Console.ReadLine();

            var sendOtp = new SendOtp()
            {
                LockerStationId = lockerStationId,
                LspId = consumerLspId,
                PhoneNumber = consumerCollectMobileNumber,
                BookingId = consumerBookingId
            };
            var sendOtpResult = gatewayService.SendOtp(sendOtp);
            Console.WriteLine("[Send Otp][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(sendOtpResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Verify Consumer Otp

            Console.WriteLine("[Consumer Verify Otp][Req]");

            Console.Write("Code: ");
            string consumerCode = Console.ReadLine();

            Console.Write("Phone Number: ");
            string ConsumerPhoneNumber = Console.ReadLine();

            Console.Write("LspId: ");
            string consumerVerifyPinLspId = Console.ReadLine();

            Console.Write("Ref Code: ");
            string consumerRefCode = Console.ReadLine();


            var consumerVerifyOtpModel = new VerifyOtp
            {
                LockerStationId = lockerStationId,
                LspId = consumerVerifyPinLspId,
                Code = consumerCode,
                PhoneNumber = ConsumerPhoneNumber,
                RefCode = consumerRefCode
            };

            var consumerVerifyOtpResult = gatewayService.VerifyOtp(consumerVerifyOtpModel);

            Console.WriteLine("[Consumer Verify Otp][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(consumerVerifyOtpResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Open Compartment

            Console.WriteLine("[Open Compartment][Req]");
            string ConsumerTransactionId = Guid.NewGuid().ToString();

            Console.Write("Locker Id: ");
            string ConsumerLockerId = Console.ReadLine();

            Console.Write("Compartment Id: ");
            string consumerCompartmentIds = Console.ReadLine();
            string[] consumerCompartmentId = consumerCompartmentIds.Split(',');

            var consumerOpenCompartment = new HCM.Core.DataObjects.Models.Compartment(ConsumerTransactionId, ConsumerLockerId, consumerCompartmentId, false, string.Empty, string.Empty);
            var consumerOpenCompartmentResult = gatewayService.OpenCompartment(consumerOpenCompartment);

            Console.WriteLine("[Open Compartment][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(consumerOpenCompartmentResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");

            #endregion

            #region Update Booking Status

            Console.WriteLine("[Update Booking Status][Req]");

            Console.Write("Booking Id: ");
            string consumerUpdateBookingStatusBookingId = Console.ReadLine();

            Console.Write("Status: ");
            string consumerStatus = Console.ReadLine();

            Console.Write("MobileNumber: ");
            string consumerMobileNumber = Console.ReadLine();

            Console.Write("Reason: ");
            string consumerUpdateBookingReason = Console.ReadLine();

            var consumerBookingStatusUpdate = new BookingStatus()
            {
                LockerStationId = lockerStationId,
                BookingId = Convert.ToInt32(consumerUpdateBookingStatusBookingId),
                LspId = consumerLspId,
                MobileNumber = consumerMobileNumber,
                Status = consumerStatus,
                Reason = consumerUpdateBookingReason

            };

            var consumerBookingStatusUpdateResult = gatewayService.UpdateBookingStatus(consumerBookingStatusUpdate);
            Console.WriteLine("[Update Booking Status][Res]");
            Console.WriteLine(JsonConvert.SerializeObject(consumerBookingStatusUpdateResult, Formatting.Indented));
            Console.WriteLine("-----------------------------------------------------------------------------");


            #endregion
        }
    }
}
