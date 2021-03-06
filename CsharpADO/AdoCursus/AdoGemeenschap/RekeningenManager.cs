﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace AdoGemeenschap
{
    public class RekeningenManager
    {
        public Int32 SaldoBonus()
        {
            var dbmanager = new BankDbManager();
            using (var conBank = dbmanager.GetConnection())
            {
                using (var comBonus = conBank.CreateCommand())
                {
                    comBonus.CommandType = CommandType.Text;
                    comBonus.CommandText = "update Rekeningen set Saldo=Saldo*1.1";
                    conBank.Open();
                    return comBonus.ExecuteNonQuery();
                }
            }

        }

        public Boolean Storten(Decimal teStorten, String rekeningnr)
        {
            var dbmanager = new BankDbManager();
            using (var conBank = dbmanager.GetConnection())
            {
                using (var comStorten = conBank.CreateCommand())
                {
                    comStorten.CommandType = CommandType.StoredProcedure;
                    comStorten.CommandText = "Storten";

                    DbParameter parTeStorten = comStorten.CreateParameter();
                    parTeStorten.ParameterName = "@teStorten";
                    parTeStorten.Value = teStorten;
                    parTeStorten.DbType = DbType.Currency;
                    comStorten.Parameters.Add(parTeStorten);

                    DbParameter parRekeningNr = comStorten.CreateParameter();
                    parRekeningNr.ParameterName = "@rekeningnr";
                    parRekeningNr.Value = rekeningnr;
                    comStorten.Parameters.Add(parRekeningNr);
                    conBank.Open();
                    return comStorten.ExecuteNonQuery() != 0;
                }
            }


        }

        public void Overschrijven(Decimal bedrag, String VanRekening, String NaarRekening)
        {
            var manager = new BankDbManager();
            //aanpassingen gebeurd cursus p 53
            var manager2 = new Bank2DbManager();

            var opties = new TransactionOptions();
            opties.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;



            using (var traOverschrijven = new TransactionScope(TransactionScopeOption.Required, opties)) //traOverschrijving en conBank zijn van plaats verwisseld
            {
                using (var conBank = manager.GetConnection())
                {
                    using (var comAftrekken = conBank.CreateCommand())
                    {

                        comAftrekken.CommandType = CommandType.Text;
                        comAftrekken.CommandText = "update Rekeningen set Saldo=Saldo-@bedrag where RekeningNr=@reknr";

                        var parBedrag = comAftrekken.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comAftrekken.Parameters.Add(parBedrag);

                        var parRekNr = comAftrekken.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = VanRekening;
                        comAftrekken.Parameters.Add(parRekNr);

                        conBank.Open();
                        if (comAftrekken.ExecuteNonQuery() == 0)
                        {

                            throw new Exception("Van rekening bestaat niet");
                        }

                    } //using comAftrekken

                    ////using conBank
                }
                using (var conBank = manager2.GetConnection())
                {


                    using (var comBijtellen = conBank.CreateCommand())
                    {

                        comBijtellen.CommandType = CommandType.Text;
                        comBijtellen.CommandText = "update Rekeningen set Saldo=Saldo+@bedrag where RekeningNr=@reknr";

                        var parBedrag = comBijtellen.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comBijtellen.Parameters.Add(parBedrag);

                        var parRekNr = comBijtellen.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = NaarRekening;
                        comBijtellen.Parameters.Add(parRekNr);

                        conBank.Open();
                        if (comBijtellen.ExecuteNonQuery() == 0)
                        {

                            throw new Exception("Naar rekening bestaat niet");
                        }
                        traOverschrijven.Complete();
                    } //using comBijtellen
                }//using traOverschrijven
            }//using conBank
        }

        public Decimal SaldoRekeningRaadplegen(String rekeningNr)
        {
            var dbManager=new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comSaldo = conBank.CreateCommand())
                {
                    comSaldo.CommandType=CommandType.StoredProcedure;
                    comSaldo.CommandText = "SaldoRekeningRaadplegen";
                    var parReknr = comSaldo.CreateParameter();
                    parReknr.ParameterName = "@rekeningNr";
                    parReknr.Value = rekeningNr;
                    comSaldo.Parameters.Add(parReknr);
                    conBank.Open();
                    Object resultaat = comSaldo.ExecuteScalar();
                    if (resultaat == null)
                    {
                        throw new Exception("Rekening bestaat niet");
                    }
                    else
                    {
                        return (Decimal) resultaat;
                    }
                }
            }
        }

        public RekeningInfo RekeningInfoRaadlplegen(string RekeningNummer)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comSaldo = conBank.CreateCommand())
                {
                    comSaldo.CommandType=CommandType.StoredProcedure;
                    comSaldo.CommandText = "RekeningInfoRaadplegen";

                    var parReknr = comSaldo.CreateParameter();
                    parReknr.ParameterName = "@RekeningNr";
                    parReknr.Value = RekeningNummer;
                    comSaldo.Parameters.Add(parReknr);

                    var parSaldo = comSaldo.CreateParameter();
                    parSaldo.ParameterName = "@Saldo";
                    parSaldo.DbType = DbType.Currency;
                    parSaldo.Direction = ParameterDirection.Output;
                    comSaldo.Parameters.Add(parSaldo);

                    var parKlantNaam = comSaldo.CreateParameter();
                    parKlantNaam.ParameterName = "@KlantNaam";
                    parKlantNaam.DbType= DbType.String;
                    parKlantNaam.Direction= ParameterDirection.Output;
                    parKlantNaam.Size = 50;
                    comSaldo.Parameters.Add(parKlantNaam);

                    conBank.Open();
                    comSaldo.ExecuteNonQuery();
                    if (parSaldo.Value.Equals(DBNull.Value))
                    {
                        throw new Exception("Rekening bestaat niet");

                    }
                    else
                    {
                        return new RekeningInfo((Decimal)parSaldo.Value,(String)parKlantNaam.Value);
                    }


                }
            }
        }
    }
}
