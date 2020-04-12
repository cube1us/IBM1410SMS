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
    #   Create the empty keys dictonary: table : {'keys' : []}
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
    #   The "dangling" dictionary tracks rows that HAVE dangling references
    #

    dangling = {}
    
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
    #   Also prepare refs[<table name>][<key value>] with 0 counts for use later
    #

    refs = {}

    for tableName in tableDict.keys():

        table = tableDict[tableName]
        refs[tableName] = {}
        dangling[tableName] = {}

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


    print("*** END OF DUPLICATE PROCESSING ***")
    print()
    sys.stdout.flush()

    #
    #   Next, for each table, add references to a reference count in other
    #   tables.
    #
    #   refs is a dictionary that counts references TO each row FROM other
    #   tables as refs[<table name>][<key value>].
    #
    #   While we are at it, we report any cases where a foreign key refers
    #   to a row in the foreign table that does not actually exist (in which
    #   case refs[<foreign table>][<foreign key value>] will not exist.
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
                    dangling[tableName][keyvalue] = True

                    #
                    #   If there is a display function for this table, display
                    #   information about this row, to the extent possible
                    #

                    funcName = "show_" + tableName
                    if(funcName in globals()):
                        globals()[funcName](keyvalue)


        sys.stdout.flush()

    print("*** END OF DANGLING REFERENCE PROCESSING ***")
    print()
    sys.stdout.flush()

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
            continue

        #
        #   Not a leaf.  Report any id values with a 0 count
        #

        for keyvalue in refs[tableName].keys():
            if(refs[tableName][keyvalue] == 0):
                print("   Table " + tableName + ", key value " + keyvalue +
                      " has nothing referring to it (orphaned)",end='')

                #
                #   Note if this orphan also sources one or more dangling refs.
                #

                if(keyvalue in dangling[tableName].keys()):
                    print(" *** Also has one or more dangling references")
                else:
                    print()

                #   Display information about this row, if possible.

                funcName = "show_" + tableName
                if(funcName in globals()):
                     globals()[funcName](keyvalue)

        sys.stdout.flush()

    #
    #   Special - check for references to BLANK ecos

    query = "SELECT idECO, machine, description FROM eco WHERE eco = ''"
    cursor.execute(query)
    for idecotuple in cursor:
        (ideco, idmachine, description) = idecotuple
        print("Checking blank ECO " + str(ideco) + ", Machine " +
             str(getMachineName(idmachine)) + ", Description: " + str(description))
        for table in tableDict.keys():
            key = tableDict[table]['key']
            for fk in tableDict[table]['fk'].keys():
                if(tableDict[table]['fk'][fk] == "eco"):
                    query2 = ("SELECT " + key + " FROM " + table +
                        " WHERE " + "`" + fk + "` = '" + str(ideco) + "'")
                    cursor2.execute(query2)
                    if(cursor2.rowcount > 0):
                        for idfktuple in cursor2:
                            fkeyvalue = idfktuple[0]
                            print("   Reference to Blank ECO from table " +
                                  table + " key " + str(fkeyvalue))
                            funcName = "show_" + table
                            if(funcName in globals()):
                                globals()[funcName](fkeyvalue)
            sys.stdout.flush()

    # 
    #   All done
    #

    cursor.close()
    cursor2.close()
    cnx.close()

#
#   Routine to display a volume Name
#
def show_volume(idVolume):
    (volumeName, machineSerial) = getVolumeInfo(idVolume)
    print("      Volume: " + str(volumeName) + ", Machine S/N: " + machineSerial)
    return
#
#   Routine to display an ECO name
#

def show_eco(idECO):
    (ecoName, machineName) = getECOInfo(idECO)
    print("      ECO: " + str(ecoName) + ", Machine: " + machineName)
    return

#
#   Routine to diplsay a Logic Function Table entry
#
def show_logicfunction(idLogicFunction):
    cursor = cnx.cursor()
    query = ("SELECT name FROM logicfunction WHERE idlogicfunction = '" +
        str(idLogicFunction) + "'")
    cursor.execute(query)
    logicFunctionName = cursor.fetchone()[0]
    cursor.close()
    print("      Logic Function: " + logicFunctionName)
    return

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
    return

def show_machinegate(idMachineGate):
    cursor = cnx.cursor()
    query = ("SELECT name, frame FROM machinegate WHERE idgate = '" +
        str(idMachineGate) + "'")
    cursor.execute(query)
    (gateName, idFrame) = cursor.fetchone()
    query = ("SELECT name, machine FROM frame WHERE idframe = '" + str(idFrame) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (frameName, idMachine) = cursor.fetchone()  
        machineName = getMachineName(idMachine)
    else:
        frameName = machineName = "(N/A)"
    cursor.close()
    print("      Machine Gate: " + str(machineName) + str(frameName) + str(gateName))

#
#   Display information about a diagram page
#
def show_diagrampage(idDiagrampage):
    cursor = cnx.cursor()
    query = ("SELECT page FROM diagrampage WHERE iddiagrampage = '" + 
        str(idDiagrampage) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        page = cursor.fetchone()[0]
        cursor.close()
        (pageName, volumeName, pageMachineName, part) = getPageInfo(page)
    else:
        pageName = "(N/A)"

    #   Special case here:  if there is no page name, this is a dangling
    #   reference with no useful information available.

    if(pageName != "(N/A)"):
        print("      Diagram Page: " + pageName + ", Volume: " + volumeName +
            ", Part# " + str(part) + ", Page Machine: " + pageMachineName)

#
#   Display information about a Diagram ECO Tag
#

def show_diagramecotag(idDiagramECOTag):
    cursor = cnx.cursor()
    query = ("SELECT name, date, diagrampage, eco FROM diagramecotag " +
             "WHERE iddiagramecotag = '" + str(idDiagramECOTag) + "'")
    cursor.execute(query)
    (tagName, tagDate, idDiagramPage, idECO) = cursor.fetchone()
    cursor.close()
    print("      ECO Tag: " + tagName + ", Date: " + str(tagDate),end=' ')
    show_diagrampage(idDiagramPage)
    show_eco(idECO)
    return

#
#   Display information about a diagram block
#
def show_diagramblock(idDiagramBlock):
    cursor = cnx.cursor()
    query = ("SELECT diagrampage, diagramrow, diagramcolumn, cardslot, cardtype, eco "+
             " FROM diagramblock where iddiagramblock = '" + str(idDiagramBlock) + "'")
    cursor.execute(query)
    (idDiagramPage, diagramRow, diagramColumn, idCardSlot, idCardType, 
        idECOTag) = cursor.fetchone()
    print("      Diagram Block: Row: " + diagramRow + ", Col: " + str(diagramColumn) +
          ", Type: " + getCardTypeName(idCardType) + ", Slot: " + 
          getCardSlot(idCardSlot),end='')
    show_diagrampage(idDiagramPage)

#
#   Display information about a Card Note
#

def show_cardnote(idCardNote):
    cursor = cnx.cursor()
    query = ("SELECT notename, note, cardtype FROM cardnote WHERE idcardnote = '" +
        str(idCardNote) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (noteName, note, idCardType) = cursor.fetchone()
        cardTypeName = getCardTypeName(idCardType)
    else:
        noteName = note = cardTypeName = "(N/A)"
    cursor.close()
    print("      Card Note: Type: " + cardTypeName + ", Name: " + noteName +
        ", Note: " + note)
    return

#
#   Display information about a card's gate
#

def show_cardgate(idCardGate):
    cursor = cnx.cursor()
    query = ("SELECT number, cardtype FROM cardgate WHERE idcardgate = '" +
        str(idCardGate) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (gateNumber, idCardType) = cursor.fetchone()
        cardTypeName = getCardTypeName(idCardType)
    else:
        gateNumber = cardTypeName = "(N/A)"
    cursor.close()
    print("      Card Gate: Type: " + cardTypeName + ", Number: " + 
        str(gateNumber))
    return
                   
#
#   Routine to display important information related to a card location
#   for tracing orphans.
#

def show_cardlocation(idCardlocation):
    cursor = cnx.cursor()
    
    query = ("SELECT page, cardslot, type FROM cardlocation WHERE idcardlocation = '" + 
        str(idCardlocation) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (clpage, cardslot, type) = cursor.fetchone()
        if(debug > 1):
            print("      show_cardlocation:  clpage=" + str(clpage) + ", cardslot=" + str(cardslot) +
                ", type=" + str(type))
    
        query = ("SELECT page from cardlocationpage WHERE idcardlocationpage = '" + 
             str(clpage) + "'")
        cursor.execute(query)
        page = cursor.fetchone()[0]
        cursor.close()
        (pageName, volumeName, machineName, part) = getPageInfo(page)
        slotName = getCardSlot(cardslot)
        cardType = getCardTypeName(type)
    else:
        pageName = slotName = cardType = "(N/A)"

    print("      Page: " + str(pageName) + ", Slot: " + str(slotName) + 
          ", Type: " + str(cardType))
    cursor.close()
    return

#
#   Display informatino about a card location block
#
def show_cardlocationblock(idCardLocationBlock):
    cursor = cnx.cursor()
    query = ("SELECT cardlocation, diagrampage, cableedgeconnectionpage, " +
        " diagramrow, diagramcolumn FROM cardlocationblock " +
        " WHERE idcardlocationblock = '" + str(idCardLocationBlock) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (idCardLocation, idDiagramPage, idCableEdgeConnectionPage, diagramRow,
            diagramColumn) = cursor.fetchone()
        print("      Card Location Page: Row " + str(diagramRow) + ", Col: " +
              str(diagramColumn) + ", ",end='')
        show_cardlocation(idCardLocation)
        if(idDiagramPage is not None):
            show_diagrampage(idDiagramPage)
        if(idCableEdgeConnectionPage is not None):
            show_cableedgeconnectinpage(idCableEdgeConnectionPage)
    cursor.close()
    return

#
#   Display information about a card location page
#   

def show_cardlocationpage(idCardLocationPage):
    cursor = cnx.cursor()
    query = ("SELECT page, eco, panel FROM cardlocationpage " +
             "WHERE idcardlocationpage ='" + str(idCardLocationPage) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (page, ideco, idpanel) = cursor.fetchone()
        (pageName, volumeName, pageMachineName, part) = getPageInfo(page)

        query = ("SELECT panel, gate FROM panel WHERE idpanel = '" + str(idpanel) + "'")
        cursor.execute(query)
        if(cursor.rowcount > 0):
            (panel, idgate) = cursor.fetchone()
            query = ("SELECT name, frame FROM machinegate WHERE idgate = '" + str(idgate) + "'")
            cursor.execute(query)
            (gate, idframe) = cursor.fetchone()
            query = ("SELECT name, machine FROM frame WHERE idframe = '" + str(idframe) + "'")
            cursor.execute(query)
            (frame, idmachine) = cursor.fetchone()
            machineName = getMachineName(idmachine)
        else:
            panel = gate = frame = machineName = "(N/A)"
        (ecoName, junk) = getECOInfo(ideco)     # Ignore the ECO machine
    else:
        pageName = panel = gate = frame = machineName = volumeName = "(N/A)"
        part = pageMachineName = ecoName = "(N/A)"

    print("      Card Location Page: " + pageName + ", Volume: " + volumeName +
          ", Part# " + str(part) + ", Page Machine: " + pageMachineName +
         ", Machine/Frame/Gate/Panel: " + str(machineName) + str(frame) + str(gate) + 
         str(panel) + ", ECO: " + ecoName)

    cursor.close()
    return

#
#   Routine to display info about a cable/edge connection page
#
def show_cableedgeconnectionpage(idCableEdgeConnectionPage):
    cursor = cnx.cursor()
    query = ("SELECT page FROM cableedgeconnectionpage " +
        "WHERE idcableedgeconnectionpage ='" + str(idCableEdgeConnectionPage) + "'")
    cursor.execute(query)
    page = cursor.fetchone()[0]
    cursor.close()
    (pageName, volumeName, pageMachineName, part) = getPageInfo(page)

    #   Special case here:  if there is no page name, this is a dangling
    #   reference with no useful information available.

    if(pageName != "(N/A)"):
        print("      Cable/Edge Page: " + pageName + ", Volume: " + volumeName +
            ", Part# " + str(part) + ", Page Machine: " + pageMachineName)

#
#   Routine to display info about a cable/edge connection ECO tag
#
def show_cableedgeconnectionecotag(idCableEdgeConnectionECOTag):
    cursor = cnx.cursor()
    query = ("SELECT name, eco, date, cableedgeconnectionpage " +
             "FROM cableedgeconnectionecotag " +
             "WHERE idcableedgeconnectionecotag = '" + 
             str(idCableEdgeConnectionECOTag) + "'")
    cursor.execute(query)
    (ecoTag, idECO, ecoDate, idCecPage) = cursor.fetchone()
    cursor.close()
    
    print("      Cable/Edge ECO Tag: " + ecoTag + ", TagDate: " + str(ecoDate) + ", ")
    show_eco(idECO)
    show_cableedgeconnectionpage(idCecPage)

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
    if(cursor.rowcount > 0):
        (pageName, idMachine, idVolume, part) = cursor.fetchone()
        machineName = getMachineName(idMachine)
        (volumeName, junk) = getVolumeInfo(idVolume)
    else:
        pageName = volumeName = machineName = part = "(N/A)"
    cursor.close()
    return (pageName, volumeName, machineName, part)

#
#   Routine to retreive the page name of a diagram page given its key
#
def getDiagramPageInfo(idDiagrampage):
    cursor = cnx.cursor()
    query = ("SELECT page FROM diagrampage WHERE iddiagrampage='" + 
         str(idDiagrampage) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        page = cursor.fetchone()[0]
    else:
        page = "(N/A)"
    cursor.close()
    return(getPageInfo(page))

#
#   Routine to get ECO Info
#
def getECOInfo(idECO):
    cursor = cnx.cursor()
    query = ("SELECT eco, machine FROM eco WHERE ideco = '" + str(idECO) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (ecoName, idmachine) = cursor.fetchone()
        machineName = getMachineName(idmachine)
    else:   
        ecoName = machineName = "(N/A)"
    cursor.close()
    return(ecoName, machineName)

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
    if(cursor.rowcount > 0):
        type = cursor.fetchone()[0]
    else:
        type = "(N/A)"
    cursor.close()
    return(type)

#
#   Routine to return the name of a machine
#

def getMachineName(idMachine):    
    cursor = cnx.cursor()
    query = ("SELECT name FROM machine WHERE idmachine = '" + str(idMachine) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        machine = cursor.fetchone()[0]
    else:
        machine = "(N/A)"
    cursor.close()
    return(machine)

#
#   Routine to return the name of a volume
#

def getVolumeInfo(idVolume):
    cursor = cnx.cursor()
    query = ("SELECT name, machineSerial FROM volume WHERE idvolume = '" + 
        str(idVolume) + "'")
    cursor.execute(query)
    if(cursor.rowcount > 0):
        (volumeName, machineSerial) = cursor.fetchone()
    else:
        volumeName = machineSerial = "(N/A)"
    cursor.close()
    return(volumeName, machineSerial)

def usage():
    print("Usage: " + sys.argv[0] + " [-v] [-d] [-u | --user <user>] [-p | --password <password>] ")
    return

if __name__ ==  "__main__":
    main()
