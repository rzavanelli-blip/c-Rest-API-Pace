function stampa() {
    fetch("https://localhost:7041/api/menu")
        .then(response => response.json())
        .then(dati => {
            const corpoTabella = document.getElementById("corpo-tabella");
            corpoTabella.innerHTML = ""; // Pulisce la tabella


        

            for (let pizza of dati) {
                
                corpoTabella.innerHTML += `
                    <tr>
                        <td>${pizza.nome}</td>
                        <td>${pizza.isVegana ? "Sì" : "No"}</td>
                        <td>${pizza.prezzo}</td>
                    
                    </tr>
                `;
            }
        })
        .catch(error => console.error("Errore nel recupero del menu:", error));
}
