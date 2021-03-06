﻿using LiteDB;
using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class CalcConfigData : ICalcConfig, IGetSignData {
        public CalcConfigData() { }

        public CalcConfigData(ICalcConfig data) {
            this.CoinCode = data.CoinCode;
            this.Speed = data.Speed;
            this.SpeedUnit = data.SpeedUnit;
            this.IncomePerDay = data.IncomePerDay;
            this.IncomeUsdPerDay = data.IncomeUsdPerDay;
            this.IncomeCnyPerDay = data.IncomeCnyPerDay;
            this.CreatedOn = data.CreatedOn;
            this.ModifiedOn = data.ModifiedOn;
        }

        public void Update(ICalcConfig data) {
            this.Speed = data.Speed;
            this.SpeedUnit = data.SpeedUnit;
            this.IncomePerDay = data.IncomePerDay;
            this.IncomeUsdPerDay = data.IncomeUsdPerDay;
            this.IncomeCnyPerDay = data.IncomeCnyPerDay;
            this.CreatedOn = data.CreatedOn;
            this.ModifiedOn = data.ModifiedOn;
        }

        [BsonId]
        public string CoinCode { get; set; }

        public double Speed { get; set; }

        public string SpeedUnit { get; set; }

        public double IncomePerDay { get; set; }

        public double IncomeUsdPerDay { get; set; }

        public double IncomeCnyPerDay { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
