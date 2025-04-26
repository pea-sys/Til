package main

import (
	"fmt"
	"log"
	"os"
	"time"
)

func printFileData(filename string) {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()
	buf := make([]byte, 1024)
	for {
		n, err := file.Read(buf)
		if n > 0 {
			fmt.Print(string(buf[:n]))
		}
		if err != nil {
			break
		}
	}

}

func main() {
	filenames := os.Args[1:]
	for _, filename := range filenames {
		go printFileData(filename)
	}
	time.Sleep(2 * time.Second)
}
