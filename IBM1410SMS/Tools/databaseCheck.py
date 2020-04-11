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
    #   Globals
    #

    global cnx
    global debug

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

    exemptFromOrphanTest = ['cardslot', 'gatepin']

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
    sys.stdout.flush()

    #
    #   Run through all of the tables, capturing the keys.
    #
    #   Also prepare refs[<table name>][<counter>] with 0 counts for use later
    #

    refs = {}

    for tableName in tableDict.keys():

        table = tableDict[tableName]
        refs[tableName] = {}

        keyName = table['key']
        if(debug or verbose):
            print("Processing dups for table " + tableName + " with key " + keyName + ", ",end=' ')
        query = "SELECT " + keyName + " FROM " + tableName
        cursor.execute(query)
        if(debug or verbose):
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
            refs[tableName][str(idvalue)] = 0
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

    sys.stdout.flush()

    #
    #   Next, for each table, add references to a refernce count in other
    #   tables.
    #
    #   refs is a dictionary that counts references TO each row FROM other
    #   tables as refs[<table name][<key value>].
    #
    #   While we are at it, we report any cases where the foreign key refers
    #   to a row in the foreign table that does not actually exist.
    #

    for tableName in tableDict.keys():
        
        table = tableDict[tableName]
        for fk in table['fk'].keys():
            otherTable = table['fk'][fk]
            if(verbose or debug):
                print("Table " + tableName + ", column " + fk +
                    ", refers to table " + otherTable)
            query = "SELECT " + table['key'] + ", `" + fk + "` FROM " + tableName
            if(debug):
                print("   " + query,end='')
            cursor.execute(query)
            if(debug):
                print(" [" + str(cursor.rowcount) + " rows]")
            for idtuple in cursor:
                (keyvalue, fkeyvalue) = idtuple
                keyvalue = str(keyvalue)
                fkeyvalue = str(fkeyvalue)
                if(fkeyvalue == "0" or fkeyvalue == str(None)):
                    continue
                if(fkeyvalue in refs[otherTable]):
                    refs[otherTable][fkeyvalue] += 1;
                else:
                    print("   Dangling reference from table " + tableName +
                          ", key " + keyvalue + ", column " + fk + 
                          ", value " + fkeyvalue + " to table " + otherTable)

        sys.stdout.flush()

    print("TEST: " + str(refs['sheetedgeinformation']['268892']))

    #
    #   Next, report any key values that have no references to them,
    #   exluding those tables that have nothing that refers to them
    #

    for tableName in tableDict.keys():
        
        table = tableDict[tableName]
        print("Processing Table " + tableName + " looking for orphans")

        if(tableName in exemptFromOrphanTest):
            print("   This table is exempt from the orphan test.")
            continue

        #
        #   Do any other tables refer to this table?
        #

        leafTable = True

        for otherTableName in tableDict.keys():
            otherTable = tableDict[otherTableName]
            for fk in otherTable['fk'].keys():
                if(otherTable['fk'][fk] == tableName):
                    leafTable = False
                    break

        if(leafTable):
            print("   This table is a LEAF table -- skipping")

        #
        #   Not a leaf.  Report any id values with a 0 count
        #

        for keyvalue in refs[tableName].keys():
            if(refs[tableName][keyvalue] == 0):
                print("Table " + tableName + ", key value " + keyvalue +
                      " has nothing referring to it (orphaned)")

    # 
    #   All done
    #

    cnx.close()

#
#   Routine to display an ECO name
#

def show_eco(idECO):
    cursor = cnx.cursor()
    query = ("SELECT eco, machine FROM eco WHERE ideco = '" + str(idECO) + "'")
    cursor.execute(query)
    (ecoName, idmachine) = cursor.fetchone()
    cursor.close()
    machineName = getMachineName(idmachine)
    print("      ECO: " + str(ecoName) + ", Machine: " + machineName)

#
#   Routine to display a feature name
#

def show_feature(idFeature):
    cursor = cnx.cursor()
    query = ("SELECT code, feature, machine FROM feature WHERE idfeature = '" + str(idFeature) + "'")
    cursor.execute(query)
    (featureName, featureDesc, idmachine) = cursor.fetchone()
    cursor.close()
    machineName = getMachineName(idmachine)
    print ("      Feature: " + featureName + ", Machine: " + machineName + 
           ", Desc: " + featureDesc)

#
#   Routine to display important information related to a card location
#   for tracing orphans.
#

def show_cardlocation(idCardlocation):
    cursor = cnx.cursor()
    
    query = ("SELECT page, cardslot, type FROM cardlocation WHERE idcardlocation = '" + 
        str(idCardlocation) + "'")
    cursor.execute(query)
    (clpage, cardslot, type) = cursor.fetchone()
    if(debug > 1):
        print("      show_cardlocation:  clpage=" + str(clpage) + ", cardslot=" + str(cardslot) +
            ", type=" + str(type))
    
    query = ("SELECT page from cardlocationpage WHERE idcardlocationpage = '" + 
         str(clpage) + "'")
    cursor.execute(query)
    page = cursor.fetchone()[0]
    (pageName, volumeName, machineName, part) = getPageInfo(page)
    slotName = getCardSlot(cardslot)
    cardType = getCardTypeName(type)

    print("      Page: " + str(pageName) + ", Slot: " + str(slotName) + 
          ", Type: " + str(cardType))
    cursor.close()
    return

#
#   Routine to display information about an orphan SheetEdgeInformation row
#

def show_sheetedgeinformation(idSheetEdgeInformation):
    cursor = cnx.cursor()
    query = ("SELECT diagrampage, `row`, signalname, leftside, rightside " +
        "FROM sheetedgeinformation WHERE idsheetedgeinformation = '" + 
        str(idSheetEdgeInformation) + "'")
    cursor.execute(query)
    (idDiagrampage, row, signalName, left, right) = cursor.fetchone()
    cursor.close()
    (diagramPageName, volumeName, machineName, part) = getDiagramPageInfo(idDiagrampage)
    side = ""
    if(left > 0):
        side = "L"
    if(right > 0):
        side = "/R"
    print("      Diagram Page: " + str(diagramPageName) + ", Page row: " + 
        str(row) + ", Side: " + side + ", Signal Name: " + str(signalName))
    return

#
#   Routine to retrieve info on a page, given its key
#
#   Returns a tuple (page name, volume name, machine name, part)
#

def getPageInfo(idPage):
    cursor = cnx.cursor()
    query = ("SELECT name, machine, volume, part FROM page WHERE idpage = '" + str(idPage) + "'")
    cursor.execute(query)
    (pageName, idMachine, idVolume, part) = cursor.fetchone()
    cursor.close()
    machineName = getMachineName(idMachine)
    volumeName = getVolumeName(idVolume)
    return (pageName, volumeName, machineName, part)

#
#   Routine to retreive the page name of a diagram page given its key
#
def getDiagramPageInfo(idDiagrampage):
    cursor = cnx.cursor()
    query = ("SELECT page FROM diagrampage WHERE iddiagrampage='" + 
         str(idDiagrampage) + "'")
    cursor.execute(query)
    page = cursor.fetchone()[0]
    cursor.close()
    return(getPageInfo(page))

#
#   Routine to retrieve the long form of a card slot (4 character machine)
#

def getCardSlot(idCardSlot):
    cursor = cnx.cursor()
    
    query = ("SELECT panel, cardrow, cardcolumn FROM cardslot WHERE idcardslot = '" + 
        str(idCardSlot) + "'")
    cursor.execute(query)
    (idpanel,row,column) = cursor.fetchone()

    query = ("SELECT panel, gate FROM panel WHERE idpanel = '" + str(idpanel) + "'")
    cursor.execute(query)
    (panel, idgate) = cursor.fetchone()

    query = ("SELECT name, frame FROM machinegate WHERE idgate = '" + str(idgate) + "'")
    cursor.execute(query)
    (gate, idframe) = cursor.fetchone()

    query = ("SELECT name, machine FROM frame WHERE idframe = '" + str(idframe) + "'")
    cursor.execute(query)
    (frame, idmachine) = cursor.fetchone()
    machine = getMachineName(idmachine)

    cursor.close()
    return(str(machine) + str(frame) + str(gate) + str(panel) + str(row) + str(column))

#
#   Routine to return the name of a card type
#

def getCardTypeName(idCardType):
    cursor = cnx.cursor()
    query = "SELECT type FROM cardtype WHERE idcardtype='" + str(idCardType) + "'"
    cursor.execute(query)
    type = cursor.fetchone()[0]
    cursor.close()
    return(type)

#
#   Routine to return the name of a machine
#

def getMachineName(idMachine):    
    cursor = cnx.cursor()
    query = ("SELECT name FROM machine WHERE idmachine = '" + str(idMachine) + "'")
    cursor.execute(query)
    machine = cursor.fetchone()[0]
    cursor.close()
    return(machine)

#
#   Routine to return the name of a volume
#

def getVolumeName(idVolume):
    cursor = cnx.cursor()
    query = ("SELECT name FROM volume WHERE idvolume = '" + str(idVolume) + "'")
    cursor.execute(query)
    volume = cursor.fetchone()[0]
    return(volume)

def usage():
    print("Usage: " + sys.argv[0] + " [-v] [-d] [-u | --user <user>] [-p | --password <password>] ")
    return

if __name__ ==  "__main__":
    main()
