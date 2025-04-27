package main

import (
	"fmt"
	"log"
	"os"
	"strings"
	"time"
)

func grepFileData(searchStr string, filename string) {

	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()
	buf := make([]byte, 1024)
	for {
		n, err := file.Read(buf)
		if n > 0 {
			if strings.Contains(string(buf[:n]), searchStr) {
				fmt.Println("Match found" + filename)
			} else {
				fmt.Println("No match found")
			}
		}
		if err != nil {
			break
		}
	}

}

func main() {
	searchStr := os.Args[1]
	filenames := os.Args[2:]
	for _, filename := range filenames {
		go grepFileData(searchStr, filename)
	}
	time.Sleep(2 * time.Second)
}
