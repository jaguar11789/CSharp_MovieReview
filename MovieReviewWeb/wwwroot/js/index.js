const backgrounds = [
    "/image/index-bg1.jpg",
    "/image/index-bg2.jpg",
    "/image/index-bg3.jpg",
    "/image/index-bg4.jpg"
];

const layer1 = document.querySelector(".bg-layer-1");
const layer2 = document.querySelector(".bg-layer-2");

let currentIndex = 0;
let showingLayer1 = true;

layer1.style.backgroundImage = `url("${backgrounds[0]}")`;
layer2.style.backgroundImage = `url("${backgrounds[1]}")`;

// 첫 번째 이미지도 천천히 확대
requestAnimationFrame(() => {
    layer1.classList.add("zoom");
});

function changeBackground() {

    currentIndex =
        (currentIndex + 1) % backgrounds.length;

    const nextImage = backgrounds[currentIndex];

    if (showingLayer1) {

        // 두 번째 레이어에 다음 이미지 준비
        layer2.style.backgroundImage =
            `url("${nextImage}")`;

        // 초기 크기
        layer2.classList.remove("zoom");

        // 브라우저가 스타일을 적용할 시간 확보
        requestAnimationFrame(() => {

            // 두 번째 이미지 등장 + 확대
            layer2.style.opacity = "1";
            layer2.classList.add("zoom");

            // 첫 번째 이미지 퇴장
            layer1.style.opacity = "0";
            layer1.classList.remove("zoom");
        });

    } else {

        // 첫 번째 레이어에 다음 이미지 준비
        layer1.style.backgroundImage =
            `url("${nextImage}")`;

        // 초기 크기
        layer1.classList.remove("zoom");

        requestAnimationFrame(() => {

            // 첫 번째 이미지 등장 + 확대
            layer1.style.opacity = "1";
            layer1.classList.add("zoom");

            // 두 번째 이미지 퇴장
            layer2.style.opacity = "0";
            layer2.classList.remove("zoom");
        });
    }

    showingLayer1 = !showingLayer1;
}

// 5초마다 다음 이미지
setInterval(changeBackground, 5000);