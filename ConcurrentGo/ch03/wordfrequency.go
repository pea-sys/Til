package main

import (
	"fmt"
	"io"
	"net/http"
	"regexp"
	"strconv"
	"strings"
)

func countWords(url string, wordFrequency map[string]int) {
	resp, _ := http.Get(url)
	defer resp.Body.Close()
	if resp.StatusCode != 200 {
		panic("Server returning error status code: " + resp.Status)
	}
	body, _ := io.ReadAll(resp.Body)

	wordRegex := regexp.MustCompile(`[a-zA-Z]+`)
	for _, word := range wordRegex.FindAllString(string(body), -1) {
		// 句読点などを削除
		word = strings.Trim(word, ".,!?;:()")
		if word != "" {
			wordFrequency[word]++
		}
	}
	fmt.Println("Completed:", url)
}

func main() {
	wordFrequency := make(map[string]int)
	for i := 1000; i <= 1030; i++ {
		url := fmt.Sprintf("https://rfc-editor.org/rfc/rfc%d.txt", i)
		countWords(url, wordFrequency)
	}
	for word, count := range wordFrequency {
		fmt.Println(word + " > " + strconv.Itoa(count))
	}
}
