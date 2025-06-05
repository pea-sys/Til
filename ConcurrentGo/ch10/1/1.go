package main

import (
	"crypto/md5"
	"crypto/sha256"
	"fmt"
	"io"
	"os"
	"path/filepath"
	"sync"
)

func FHash(filepath string) []byte {
	file, _ := os.Open(filepath)
	defer file.Close()

	sha := sha256.New()
	io.Copy(sha, file)

	return sha.Sum(nil)
}
func main() {
	dir := os.Args[1]
	files, _ := os.ReadDir(dir)
	hMd5 := md5.New()
	var prev, next *sync.WaitGroup
	for _, file := range files {
		if !file.IsDir() {
			next = &sync.WaitGroup{}
			next.Add(1)
			go func(filename string, prev, next *sync.WaitGroup) {
				fpath := filepath.Join(dir, filename)
				fmt.Println("Processing", fpath)
				hashOnFile := FHash(fpath)
				// If not the first iteration
				if prev != nil {
					prev.Wait()
				}
				hMd5.Write(hashOnFile)
				next.Done()
			}(file.Name(), prev, next)
			prev = next
		}
	}
	next.Wait()
	fmt.Printf("%s - %x\n", dir, hMd5.Sum(nil))
}
