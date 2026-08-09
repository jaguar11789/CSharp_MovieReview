let currentProvider = null;

async function loadMyPage() {
    try {
        const response = await fetch("https://localhost:7226/api/User/mypage",
            {
                method: "GET",
                credentials: "include"
            });
        if (response.status === 401) {

            alert("로그인이 필요합니다.");
            window.location.href = "/accounts/login";

            return;
        }
        if (!response.ok) {

            throw new Error("마이 페이지 정보를 가져오지 못했습니다.");
        }
        const data = await response.json();
        currentProvider = data.provider ?? null;

        const gender = data.gender === "M" ? "남성" : data.gender === "F" ? "여성" : "-";

        // 프로필
        document.getElementById("avatarText").textContent    = (data.userName || "U").charAt(0);
        document.getElementById("userName").textContent      = data.userName ?? "-";
        document.getElementById("email").textContent         = data.email ?? "-";
        document.getElementById("userId").textContent        = data.userId ?? "-";

        // 회원 정보
        document.getElementById("infoUserName").textContent  = data.userName ?? "-";
        document.getElementById("infoEmail").textContent     = data.email ?? "-";
        document.getElementById("phoneNumber").textContent   = data.phoneNumber ?? "-";
        document.getElementById("gender").textContent        = gender;
        document.getElementById("birthDate").textContent     = formatDate(data.birthDate);

        document.getElementById("zipCode").textContent       = data.zipCode ?? "-";
        document.getElementById("baseAddress").textContent   = data.baseAddress ?? "-";
        document.getElementById("detailAddress").textContent = data.detailAddress ? ` ${data.detailAddress}` : "";
        document.getElementById("createdAt").textContent     = formatDate(data.createdAt);

        // 자체 로그인 / 소셜 로그인
        document.getElementById("provider").textContent      = data.provider ?? "자체 로그인";

        console.log(data);
        console.log("Provider : ", data.provider);

    } catch (error) {
        console.error(error);
        alert("마이페이지 정보를 불러오는 중 오류가 발생했습니다.");
    }
    
}

function formatDate(value) {
    if (!value) {
        return "-";
    }
    return value.substring(0, 10);
}

function goToUserUpdate() {
    window.location.href = "/accounts/user-update";
}

function goToPasswordChange() {
    if (currentProvider) {
        alert(`${currentProvider} 소셜 로그인 계정은 비밀번호 변경을 지원하지 않습니다.`);
        return;
    }
    window.location.href = "/accounts/password-change";
}

document.addEventListener("DOMContentLoaded", loadMyPage);