package main

import (
	"bufio"
	"database/sql"
	"flag"
	"fmt"
	"log"
	"os"
	"strings"
	"time"

	_ "github.com/mattn/go-sqlite3"
)

var db *sql.DB

func main() {

	var dbFileNameCommandPtr = flag.String("filename", "pexip.db", "Specify the database filename.")
	var countRecordsCommandPtr = flag.Bool("count", false, "Returns the number of records in each table. If duration is provided returns the number of records beyond that duration.")
	var deleteRecordsCommandPtr = flag.Bool("delete", false, "Delete records. If duration is provided deletes records beyond that point, else all records are deleted.")
	var durationToKeepCommandPtr = flag.Duration("duration", 0, "The desired duration in hours. Use the format 0h")

	flag.Parse()

	initDb(*dbFileNameCommandPtr)

	if *countRecordsCommandPtr {
		deleteRecordsOlderThanPoint := time.Now().Add(-*durationToKeepCommandPtr)
		fmt.Println("Total records in table ConferenceHistory:", getRecordCountWithTimestamp("ConferenceHistory", deleteRecordsOlderThanPoint.Unix()))
		fmt.Println("Total records in table MediaStreamHistory:", getRecordCountWithTimestamp("MediaStreamHistory", deleteRecordsOlderThanPoint.Unix()))
		fmt.Println("Total records in table ParticipantHistory:", getRecordCountWithTimestamp("ParticipantHistory", deleteRecordsOlderThanPoint.Unix()))
	}

	if *deleteRecordsCommandPtr {
		// prompt for confirmation
		reader := bufio.NewReader(os.Stdin)

		deleteRecordsOlderThanPoint := time.Now().Add(-*durationToKeepCommandPtr)

		if *durationToKeepCommandPtr == time.Duration(0) {
			fmt.Println("You have not specified a duration, so all records will be deleted")
		} else {
			fmt.Println("This will delete any records older than", deleteRecordsOlderThanPoint)
		}

		fmt.Println()
		fmt.Print("Are you sure (y/n): ")

		text, _ := reader.ReadString('\n')

		if strings.TrimSuffix(strings.ToLower(text), "\n") != "y" {
			// user exit
			os.Exit(0)
		}

		epochSeconds := deleteRecordsOlderThanPoint.Unix()

		fmt.Println()
		fmt.Println("Deleting records with a timestamp older than:", epochSeconds)

		deleteRecords(epochSeconds, "ConferenceHistory")
		deleteRecords(epochSeconds, "MediaStreamHistory")
		deleteRecords(epochSeconds, "ParticipantHistory")
	}
}

func deleteRecords(epochTimestamp int64, tableName string) {
	var err error

	fmt.Println("Records prior to deletion in table", tableName, ":", getRecordCount(tableName))

	stmt := fmt.Sprintf("DELETE FROM %v WHERE StartTime <= %v", tableName, epochTimestamp)

	_, err = db.Exec(stmt)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Records post deletion in table", tableName, ":", getRecordCount(tableName))
}

func getRecordCount(tableName string) int {
	sqlStatement := fmt.Sprintf("SELECT COUNT(*) as count FROM %v", tableName)

	var count int
	err := db.QueryRow(sqlStatement).Scan(&count)
	if err != nil {
		log.Fatal(err)
	}

	return count
}

func getRecordCountWithTimestamp(tableName string, epochTimestamp int64) int {
	sqlStatement := fmt.Sprintf("SELECT COUNT(*) as count FROM %v WHERE StartTime <= %v", tableName, epochTimestamp)

	var count int
	err := db.QueryRow(sqlStatement).Scan(&count)
	if err != nil {
		log.Fatal(err)
	}

	return count
}

func initDb(filename string) {
	var err error
	db, err = sql.Open("sqlite3", filename)
	if err != nil {
		log.Panic(err)
	}

	if err = db.Ping(); err != nil {
		log.Panic(err)
	}
}
