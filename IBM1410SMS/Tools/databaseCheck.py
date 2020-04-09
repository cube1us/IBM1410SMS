# 
#  COPYRIGHT 2020 Jay R. Jaeger
#  
#  This program is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#
#  This program is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  (file COPYING.txt) along with this program.  
#  If not, see <https://www.gnu.org/licenses/>.
#

#	This Python script reads thru the database and performs two checks
#	   1.  Checks for duplicate database key numbers (ID's)
#      2.  Checks for referential integrity
#

import sys
import mysql.connector
import getopt


def main():

    #
    #   A list of tables, their keys and their foreign keys 
    #

    #
    #   Format  'table' : {'key' : '<field'>, 'fk' : {'<local field>' : '<foreign table>', ...}}
    #   (Look up the key value for the foreign table using its name)
    #


    tableDict = {
        'volumeset' : {'key' : 'idvolumeset', 'fk' : {} },
        'volume' : {'key' : 'idvolume', 'fk' : {'set' : 'volumeset'}},
        'machine' : {'key': 'idmachine', 'fk' : {}},
        'frame' : {'key' : 'idframe', 'fk' : {'machine' : 'machine'}},
        'machinegate' : {'key' : 'idgate', 'fk' : {'frame' : 'frame'}},
        'panel' : {'key' : 'idpanel', 'fk' : {'gate' : 'machinegate'}},
        'cardslot' : {'key' : 'idcardslot', 'fk' : {'panel' : 'panel'}},
        'feature' : {'key' : 'idfeature', 'fk' : {'machine' : 'machine'}},
        'eco' : {'key' : 'ideco', 'fk' : {'machine' : 'machine'}},
        'page' : {'key' : 'idpage', 'fk' : {'machine' : 'machine', 'volume' : 'volume'}},
        'cardlocationpage' : {'key' : 'idcardlocationpage', 
            'fk' : {'page' : 'page', 'eco' : 'eco', 'panel' : 'panel', 'previouseco' : 'eco'}},
        'cardlocation' : { 'key' : 'idcardlocation',
            'fk' : {'page' : 'cardlocationpage', 'cardslot' : 'cardslot',
                    'type': 'cardtype', 'feature' : 'feature'}},
        'cardlocationbottomnote' : {'key' : 'idcardlocationbottomnote', 
            'fk' : {'cardlocation' : 'cardlocation'}},
        'cardlocationblock' : {'key' : 'idcardlocationblock',
            'fk' : {'cardlocation' : 'cardlocation', 'diagrampage' : 'diagrampage',
                    'cableedgeconnectionpage' : 'cableedgeconnectionpage',
                    'diagrameco' : 'eco'}},
        'diagrampage' : {'key' : 'iddiagrampage', 'fk': {'page' : 'page'}},
        'diagramecotag' : {'key' : 'iddiagramecotag', 
            'fk' : {'diagrampage' : 'diagrampage', 'eco' : 'eco'}},
        'diagramblock' : {'key' : 'iddiagramblock', 
            'fk' : {'extendedto' : 'diagramblock', 'diagrampage' : 'diagrampage',
               'feature' : 'feature', 'inputmode' : 'logiclevels',
               'outputmode' : 'logiclevels', 'cardslot' : 'cardslot', 'eco' : 'diagramecotag',
               'cardtype' : 'cardtype', 'cardgate' : 'cardgate'}},
        'dotfunction' : {'key' : 'iddotfunction', 'fk' : {'diagrampage' : 'diagrampage'}},
        'sheetedgeinformation' : {'key': 'idsheetedgeinformation', 
            'fk' : {'diagrampage' : 'diagrampage'}},
        'tiedown' : {'key' : 'idtiedown', 'fk' : {'diagrampage' : 'diagrampage',
            'cardtype' : 'cardtype', 'cardslot' : 'cardslot'}},
        'connection' : {'key' : 'idconnection', 'fk' : {'fromdiagramblock' : 'diagramblock',
            'fromdotfunction' : 'dotfunction', 'fromedgesheet' : 'sheetedgeinformation',
            'fromedgeoriginsheet' : 'sheetedgeinformation',
            'todiagramblock' : 'diagramblock', 'todotfunction' : 'dotfunction',
            'toedgesheet' : 'sheetedgeinformation', 
            'toedgedestinationsheet' : 'sheetedgeinformation'}},
        'edgeconnector' : { 'key' : 'idedgeconnector', 'fk' : {'diagrampage' : 'diagrampage',
            'cardslot' : 'cardslot'}},
        'logiclevels' : { 'key' : 'idlogiclevels', 'fk' : {}},
        'logicfamily' : {'key' : 'idlogicfamily', 'fk' : {}},
        'ibmlogicfunction' : {'key' : 'idibmlogicfunction', 'fk' : {}},
        'logicfunction' : {'key' : 'idlogicfunction' , 'fk' : {}},
        'cardtype' : {'key' : 'idcardtype', 'fk' : {'volume' : 'volume',
            'logicfamily' : 'logicfamily'}},
        'cardeco' : {'key' : 'idcardtypeeco', 'fk' : {'eco' : 'eco', 'cardtype' : 'cardtype',
            'note' : 'cardnote'}},
        'cardnote' : {'key' : 'idcardnote', 'fk' : {'cardtype' : 'cardtype'}},
        'cardgate' : {'key' : 'idcardgate', 'fk' : {'cardtype' : 'cardtype', 
            'definingpin' : 'gatepin', 'positivelogicfunction' : 'ibmlogicfunction',
            'negativelogicfunction' : 'ibmlogicfunction', 'logicfunction' : 'logicfunction',
            'latchgate' : 'cardgate', 'inputlevel' : 'logiclevels', 
            'outputlevel' : 'logiclevels'}},
        'gatepin' : {'key' : 'idgatepin', 'fk' : {'cardgate' : 'cardgate',
            'inputgate' : 'cardgate', 'outputgate' : 'cardgate'}},
        'cableedgeconnectionpage' : {'key' : 'idcableedgeconnectionpage', 
            'fk' : {'page' : 'page'}},
        'cableedgeconnectionblock' : {'key' : 'idcableedgeconnectionblock',
            'fk' : {'cableedgeconnectionpage' : 'cableedgeconnectionpage',
            'ecotag' : 'cableedgeconnectionecotag', 'cardslot' : 'cardslot',
            'destination' : 'cardslot'}},
        'cableedgeconnectionecotag' : {'key' : 'idcableedgeconnectionecotag', 
            'fk' : {'cableedgeconnectionpage' : 'cableedgeconnectionpage', 'eco' : 'eco'}},
        'cableimplieddestinations' : {'key' : 'cablesource', 'fk' : {}}
    }

    database = "ibm1410sms"
    dbuser = "collection"
    dbpw = "twiddle"

    verbose = 0
    debug = 0

    try:
        options, args = getopt.getopt(sys.argv[1:],"dvu:p:", ["user=", "password="])
    except getopt.GetoptError as err:
        print(str(err))
        usage()
        sys.exit(2)

    for option, argument in options :
       if option == "-v":
            verbose += 1
       elif option == "-d":
            debug += 1
       elif option in ("-u", "--user"):
            dbuser = argument
       elif option in ("-p", "--password"):
            dbpw = argument
       else:
            usage()
            exit

    if(debug > 1):
        for table in tableDict.keys():
            print("Table: " + table, end = ' ')
            print("key: " + tableDict[table]['key'], end = ' ')
            fk = tableDict[table]['fk']
            if(len(fk) > 0):
                print("Foreign Keys/Tables:", end = ' ')
                first = True
                for key in fk.keys():
                    if(not first):
                        print(",", end = ' ')
                    first = False 
                    print(key + "/" + fk[key], end = '')
            print('')

    #
    #   Create the empty keys dictonary:    table : {'keys' : []}
    #
    #   This holds the list of valid keys for each table.
    #

    keys = {}
    for table in tableDict.keys():
        keys[table] = []

    #
    #   The "allkeys" dictionary relates a given key value to a table
    #

    allkeys = {}
    
    #
    #   Connect to the database

    cnx = mysql.connector.connect(
        user=dbuser,
        password=dbpw,
        host='127.0.0.1',
        database=database,
        buffered=True)

    cursor = cnx.cursor()
    cursor2 = cnx.cursor()

    #
    #   Run through all of the tables, capturing the keys
    #

    for tableName in tableDict.keys():
        table = tableDict[tableName]
        keyName = table['key']
        if(debug or verbose > 0):
            print("Processing dups for table " + tableName + " with key " + keyName + ", ",end=' ')
        query = "SELECT " + keyName + " FROM " + tableName
        cursor.execute(query)
        if(debug or verbose > 0):
            print(" [" + str(cursor.rowcount) + " rows.]")
        keys[tableName] = []

        for idtuple in cursor:

            idvalue = idtuple[0]

            #
            #   Add this id number to the list of id numbers for this table
            #   and check to see if some other table is using it as well.
            #
            #   If not there, create an empty list of table names
            #

            keys[tableName].append(idvalue)
            if idvalue in allkeys:
                print("Duplicate ID value " + str(idvalue) + " in tables: " + tableName,end='')
                for othertable in allkeys[idvalue]:
                    print(", " + othertable,end='')
                print()
            else:
               allkeys[idvalue] = []

            #  
            #   Remember that this id number is  used by this table
            #
            allkeys[idvalue].append(tableName)


#
#   Next, we check for *missing* id numbers
#

    for tableName in tableDict.keys():
        table = tableDict[tableName]
        if(debug or verbose > 0):
            print("Processing missing fk entries for table " + tableName)
        for fk in table['fk'].keys():
            ftable = table['fk'][fk]
            if(debug or verbose > 0):
                print("   Processing fk named " + fk + " into table " + ftable,end=' ')
            query = "SELECT " + table['key'] + ", `" + fk + "`" + " FROM " + tableName
            if(debug or verbose > 0):
                print()
                print("   " + query,end=' ')
            cursor.execute(query)
            if(debug or verbose > 0):
                print(" [" + str(cursor.rowcount) + " rows.]")

            #
            #   Now, check that each of these id values actually exists in the foreign table
            #

            for fktuple in cursor:
                keyvalue = fktuple[0]
                fkvalue = fktuple[1]
                if(debug > 1):
                    print("Tuple: " + str(fktuple) + ", Key: " + 
                          str(keyvalue) + ", fk: " + str(fkvalue))
                if(fkvalue == None or fkvalue == 0):
                    continue
                if(fkvalue not in keys[ftable]):

                    #
                    #   Uh oh.  Found a key value that doesn't actually exist.
                    #

                    print("Error: Table: " + tableName + ", ID: " + str(keyvalue) + 
                          ", fk: " + fk + ", into f. table " + ftable + 
                          ", MISSING FK value " + str(fkvalue))

                    #   
                    #   Most often these are "orphans" where something got deleted
                    #   but the thing that pointed to it didn't.  So, check for
                    #   references to THIS row's key value.
                    #

                    refCount = 0

                    for otherTableName in tableDict.keys():
                        otherTable = tableDict[otherTableName]
                        otherKeyName = otherTable['key']
                        for otherFK in otherTable['fk'].keys():
                            if(otherTable['fk'][otherFK] == tableName):
                                if(verbose or debug > 0):
                                    print("   Table " + otherTableName + " references table " +
                                          tableName + " via field " + otherFK)
                                query = ("SELECT " + otherKeyName + " FROM " + otherTableName +
                                    " WHERE " + otherFK + " = '" + str(keyvalue) + "'")
                                if(verbose or debug > 0):
                                    print("      " + query,end='')                                
                                cursor2.execute(query)
                                if(verbose or debug > 0):
                                   print(" [" + str(cursor2.rowcount) + " rows.]")
                                refCount += cursor2.rowcount
                    print("   There are " + str(refCount) + " references to THIS row in table " +
                          tableName)
# 
#   All done
#

    cnx.close()

def usage():
    print("Usage: " + sys.argv[0] + " [-v] [-d] [-u | --user <user>] [-p | --password <password>] ")

if __name__ ==  "__main__":
    main()
