using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    public class RequestFileSend: BaseRequest
    {

        /// <summary>
        /// Full Path name of the file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File content
        /// </summary>
        public byte[] FileContents { get; set; }

        /// <summary>
        /// attributes of the file
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// when was the file created
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// name of the machine where the file is coming from
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// path to parent directory of the file
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// extension or type of file
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// read only flag
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// last time the file was accessed
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// last time the file changed
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// length of the file
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Operating system where the file is stored
        /// </summary>
        public string OperatingSystemVersion { get; set; }

        /// <summary>
        /// User name who submitted the file
        /// </summary>
        public string UserName { get; set; }

    }
}
