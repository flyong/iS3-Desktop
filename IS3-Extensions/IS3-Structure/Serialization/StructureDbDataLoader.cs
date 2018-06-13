using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

using IS3.Core;
using IS3.Core.Serialization;

namespace IS3.Structure.Serialization
{
    #region Copyright Notice
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    #endregion

    // Summary:
    //    Monitoring Db data loader
    class StructureDbDataLoader : DbDataLoader
    {

        public StructureDbDataLoader()
        { }
        public bool ReadPillar(
           DGObjects objs,
           string tableNameSQL,
           string conditionSQL,
           string orderSQL)
        {
            try
            {
                _ReadSoilPillar(objs, tableNameSQL, conditionSQL,
                    orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadSoilPillar(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;

                Pillar soilProp = new Pillar(reader);
                soilProp.id = ReadInt(reader, "ObjID").Value;
                soilProp.name = ReadString(reader, "Name");
                soilProp.fullName = ReadString(reader, "ID");
                objs[soilProp.key] = soilProp;
            }
        }


    }
}
