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

        document.getElementById("userId").value        = data.userId ?? "";
        document.getElementById("userName").value      = data.userName ?? "";
        document.getElementById("email").value         = data.email ?? "";
        document.getElementById("phoneNumber").value   = data.phoneNumber ?? "";
        document.getElementById("gender").value        = data.gender ?? "";

        document.getElementById("birthDate").value     = formatDate(data.birthDate);
        document.getElementById("zipCode").value       = data.zipCode ?? "";
        document.getElementById("baseAddress").value   = data.baseAddress ?? "";
        document.getElementById("detailAddress").value = data.detailAddress ? ` ${data.detailAddress}` : "";

        console.log(data);
    } catch (error) {

        console.log(error);
        alert("마이페이지 정보를 불러오는 중 오류가 발생했습니다.");
    }
}

function formatDate(value) {
    if (!value) {
        return "-";
    }
    return value.substring(0, 10);
}

function goToMyPage() {
    window.location.href = "/accounts/mypage";
}

document.addEventListener("DOMContentLoaded", loadMyPage);

function addr_search() {
    new daum.Postcode({
        oncomplete: function (data) {
            console.log(data);

            const roadAddr  = data.roadAddress;
            const jibunAddr = data.jibunAddress;

            document.getElementById("zipCode").value = data.zonecode;

            if (roadAddr !== '') {
                document.getElementById('baseAddress').value = roadAddr;
            } else if (jibunAddr !== '') {
                document.getElementById('baseAddress').value = jibunAddr;
            }
        }
    }).open();
}

const form   = document.getElementById("userUpdateForm");
const button = document.getElementById("updateButton");

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const userName       = document.getElementById("userName").value;
    const email          = document.getElementById("email").value;
    const phoneNumber    = document.getElementById("phoneNumber").value;
    const gender         = document.getElementById("gender").value;
    const birthDateValue = document.getElementById("birthDate").value;

    const zipCode        = document.getElementById("zipCode").value;
    const baseAddress    = document.getElementById("baseAddress").value;
    const detailAddress  = document.getElementById("detailAddress").value;

    const birthDate = birthDateValue === "" ? null : birthDateValue;

    console.log(userName);
    console.log(email);
    console.log(phoneNumber);
    console.log(gender);
    console.log(birthDate);

    console.log(zipCode);
    console.log(baseAddress);
    console.log(detailAddress);

    button.disabled = true;
    button.textContent = "변경 중...";

    try {
        const response = await fetch("https://localhost:7226/api/User/user", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify({
                userName     : userName,
                email        : email,
                phoneNumber  : phoneNumber,
                gender       : gender,
                birthDate    : birthDate,

                zipCode      : zipCode,
                baseAddress  : baseAddress,
                detailAddress: detailAddress
            })
        });
        console.log("birthDate =", birthDate);
        console.log("typeof =", typeof birthDate);
        const data = await response.json();

        if (!response.ok || data.retVal !== 0) {
            alert(`CODE : ${data.retVal ?? response.status}\n` + `"${data.retMsg ?? "회원정보 수정에 실패했습니다."}"`);

            return;
        }
        alert("회원 정보를 수정하였습니다.");

        window.location.href = "/accounts/mypage";
    }
    catch (error) {
        console.log("회원정보 수정 오류: ", error);

        alert("서버와 통신하는 중 오류가 발생했습니다.");
    }
    finally {
        button.disabled = false;
        button.textContent = "회원정보 수정";
    }
});