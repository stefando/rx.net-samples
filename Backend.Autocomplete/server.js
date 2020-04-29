const http = require("http");
const url = require("url");
const names = ["Anton", "Andrej", "Braňo", "Boris", "Denis", "Emil", "Fero", "Gabriel", "Gustáv", "Ivan", "Ján", "Jozef", "Juraj", "Kamil", "Karol", "Ladislav", "Lukáš", "Marek", "Martin", "Matúš", "Milan", "Noro" ];

http.createServer(function (req, res) {
    const parsed = url.parse(req.url, true);
    const term = parsed.query.term;
    if (!term) {
        res.statusCode = 400;
        res.end("Missing term query parameter");
        return;
    }
    setTimeout(() => {
        if (term == "x") {
            res.statusCode = 500;
            res.end("Autocomplete server failure");
            return;
        }
        res.writeHead(200, { "Content-Type": "application/json; charset=utf-8" });
        const response = names.filter(x => x.toLowerCase().startsWith(term.toLowerCase()));
        res.end(JSON.stringify(response));
    }, 3000);
}).listen(1337);
