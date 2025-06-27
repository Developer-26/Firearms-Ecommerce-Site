// Function to slide left
function slideLeft(categoryKey) {
    const container = document.getElementById(`gallery-${categoryKey}`);
    const itemWidth = container.querySelector('.gallery-item').offsetWidth;
    container.scrollBy({
        left: -itemWidth * 3, // Adjust based on the number of items to scroll
        behavior: 'smooth'
    });
}

// Function to slide right
function slideRight(categoryKey) {
    const container = document.getElementById(`gallery-${categoryKey}`);
    const itemWidth = container.querySelector('.gallery-item').offsetWidth;
    container.scrollBy({
        left: itemWidth * 3, // Adjust based on the number of items to scroll
        behavior: 'smooth'
    });
}


function openPopup(name, description, price, imagePath) {
    document.getElementById('popupTitle').innerText = name;
    document.getElementById('popupDescription').innerText = description;
    document.getElementById('popupPrice').innerText = price;
    document.getElementById('popupImage').src = imagePath;

    document.getElementById('productPopup').style.display = 'flex';
}

function closePopup() {
    document.getElementById('productPopup').style.display = 'none';
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.gallery-item, .special-item').forEach(item => {
        item.addEventListener('click', function () {
            const name = this.getAttribute('data-name');
            const description = this.getAttribute('data-description');
            const price = this.getAttribute('data-price');
            const imagePath = this.getAttribute('data-image');

            openPopup(name, description, price, imagePath);
        });
    });
});
