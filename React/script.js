function stampa() {
  fetch("https://localhost:7041/api/menu")
    .then(response => response.json())
    .then(dati => {
      const corpo = document.getElementById("corpo-tabella");
      corpo.innerHTML = ""; // pulizia iniziale

      for (let pizza of dati) {
        const listIngredienti = pizza.ingredientes
          .map(i => i.nome)
          .join(", ");

        corpo.innerHTML += `
          <tr id="pizza-${pizza.id}">
            <td>${pizza.nome}</td>
            <td>${pizza.isVegana}</td>
            <td>${pizza.prezzo}</td>
            <td>${listIngredienti}</td>
            <td>
              <button onclick="eliminaPizza(${pizza.id})">Elimina</button>
            </td>
          </tr>
        `;
      }
    });
}

// 🔥 Questa funzione deve stare fuori da stampa()
function eliminaPizza(id) {
  console.log("Chiamata DELETE per pizza ID:", id);

  fetch(`https://localhost:7041/api/menu/${id}`, {
    method: 'DELETE'
  })
    .then(response => {
      if (response.ok) {
        const riga = document.getElementById(`pizza-${id}`);
        if (riga) riga.remove();
      } else {
        console.error("Errore nella cancellazione");
      }
    })
    .catch(error => {
      console.error("Errore di rete:", error);
    });
}

stampa(); // chiama la funzione all'avvio
